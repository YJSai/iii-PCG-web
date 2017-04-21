using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;
using PagedList;
using Combination0608.ViewModels;
using System.Data.Entity;
using Hangfire;
using System.Web.Security;

namespace Combination0608.Controllers {
    public class AttSController : Controller {
        private PCGEntities _db = new PCGEntities();

        private const int PageSize = 10;    //初始設定頁面最多10頁
        int Fac;
        int ZId;

        private IEnumerable<Zone> ZoneID
        {   //地區下拉式選單初始值
            get { return _db.Zone.OrderBy(x => x.ZoneID); }
        }

        private IEnumerable<AttS> Att
        {   //搜尋結果初始值
            get {
                return from A in _db.Administrators
                       join F in _db.Factories on A.FacID equals F.FacID
                       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                       where Z.ZoneID == ZId && F.FacNo == Fac
                       orderby Z.ZoneID
                       //select new { ZID = Z.Country, FacName = F.FacName, Character = A.Character, EmpName = A.Name, EmpEmail = A.Email };
                       select new AttS { Country = Z.Country, FacName = F.FacName, Character = A.Character, EmpName = A.Name, EmpEmail = A.Email };
            }
        }

        private IEnumerable<Factories> Factory(int? ZoneId) {
            //下拉式選擇之後，會傳回 ZoneId 去搜尋
            { return _db.Factories.Where(x => x.ZoneID == ZoneId); }
        }

        public JsonResult FactoryName(string CityID) {  //從 VIEW 的 AJAX 傳回來的方法
            var items = new List<KeyValuePair<string, string>>();
            //產品
            int categoryId = 0;
            if (int.TryParse(CityID, out categoryId)) { //把 CityID 轉成 categoryId
                var couties = this.GetFactoryByCity(categoryId);
                //呼叫 GetFactoryByCity 傳入 categoryId
                foreach (var county in couties) {
                    items.Add(new KeyValuePair<string, string>(county.Key, county.Value));
                    //再把抓到的 FacID 跟 FacName 放進 KeyValuePair
                }
            }
            return this.Json(items);
        }

        private Dictionary<string, string> GetFactoryByCity(int ZoneID) {
            //從 FactoryName 接收到 ZoneID 後，用 LINQ 搜尋 廠區
            var query = _db.Factories
                          .Where(x => x.ZoneID == ZoneID)
                          .Select(
                              x => new
                              {
                                  FacID = x.FacNo,
                                  FacName = x.FacName
                              })
                          .OrderBy(x => x.FacID);

            return query.ToDictionary(x => x.FacID.ToString(), x => x.FacName);
            //把廠區的 FacID 跟 FacName 放進 Dictionary
        }

        // GET: 首頁
        [Authorize]
        public ActionResult Index(int page = 1) {

            //登入時取得session empolyeeID
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                int userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);

                PCGEntities db = new PCGEntities();

                var query2 = (from ad in db.Administrators//使用者名稱SESSION
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query2;
                Session.Add("Name", name);
          
            if (userdata == 5) { 

            var query = _db.Factories.OrderBy(x => x.ZoneID);
            int pageIndex = page < 1 ? 1 : page;

            var model = new FactoryListViewModel
            {
                SearchParaMeter = new FactorySearchViewModel(),
                PageIndex = pageIndex,
                ZoneID = new SelectList(this.ZoneID, "ZoneID", "Country"),  //地區
                Factory = new SelectList(this.Factory(null), "FacNo", "FacName"),//廠區
                AttSearchList = this.Att.ToPagedList(pageIndex, PageSize)   //AttSearchList = ?
            };
            return View(model);
            }
            else {
                    Session.Add("Name", "Guset");
                    return RedirectToAction("NoPermission");
                }
            }
            return View();
        }

        //按下搜尋按鈕
        [HttpPost]
        public ActionResult Index(FactoryListViewModel model) {
            //從資料庫搜尋 Administrators/Factories/Zone 資料表的資料 放進 AttS
            //int empID = Convert.ToInt32(Session["EmployeeID"].ToString());
            //if (empID == 5) {
            //    //int.TryParse(sn, out empID);

            //var query = from P in _db.Permission
            //            join F in _db.Factories on P.FacNo equals F.FacNo
            //            join A in _db.Administrators on F.FacID equals A.FacID
            //            join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                        //where P.EmployeeID == empID
            //以上權限控管暫時不需要
            var query = from F in _db.Factories
                        join A in _db.Administrators on F.FacID equals A.FacID
                        join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                        orderby Z.ZoneID
                        select new AttS { FacNo = F.FacNo, ZoneID = Z.ZoneID, Country = Z.Country, FacName = F.FacName, Character = A.Character, EmpName = A.Name, EmpEmail = A.Email };
                        //把需要用到的資料都塞進去
            
            //如果 ZoneID 不等於空值或空白 AND ZoneID 轉成 int ZId 然後 query =  用 ZId 比對 ZoneID
            if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.ZoneID)
                &&
                int.TryParse(model.SearchParaMeter.ZoneID, out ZId)) {
                query = query.Where(x => x.ZoneID == ZId);
            }
            //如果 Factory 不等於空值或空白 AND Factory 轉成 int Fac 然後 query =  用 Fac 比對 Factory
            if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.Factory)
                &&
                int.TryParse(model.SearchParaMeter.Factory, out Fac)) {   /*model.search = Fac;*/
                query = query.Where(x => x.FacNo == Fac);
            }
            
            query = query.OrderBy(x => x.ZoneID);
            int pageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;
            var result = new FactoryListViewModel
            {
                SearchParaMeter = model.SearchParaMeter,
                ZoneID = new SelectList(    //回傳地區下拉式選單
                        items: this.ZoneID, dataValueField: "ZoneID",
                        dataTextField: "Country",
                        selectedValue: model.SearchParaMeter.ZoneID),
                Factory = new SelectList(   //回傳廠區下拉式選單
                        items: this.Factory(ZId), dataValueField: "FacNo",
                        dataTextField: "FacName",
                        selectedValue: model.SearchParaMeter.Factory),//
                PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex,  //回傳頁數

                AttSearchList = query.ToPagedList(pageIndex, PageSize)
            };
       
            return View(result);
            //}
            //else {
            //    return RedirectToRoute("Login");
            //}
        }

        //按下手動送出
        public JsonResult GetEmail(string Email) {  //從 VIEW 的 AJAX 傳回來的方法
            //var email = Email;
            var email = "taipeibigegg@gmail.com";
            string Sended = "已寄出";
            CheckEmp ce = new CheckEmp();
            BackgroundJob.Enqueue(() => ce.Check(email));
            return this.Json(Sended);
        }

        public ActionResult NoPermission() {

            return View();
        }

        public ActionResult Option() {

            return View();
        }

        [HttpPost]
        public ActionResult Option(string title, string body) {
            //CheckEmp ce = new CheckEmp();
            //BackgroundJob.Enqueue(() => ce.Check(title, body));
            TempData["mailt"] = title;
            TempData["mailb"] = body;
            if (TempData["mailt"] != null) { 
            return RedirectToAction("index");
            } else {
                return RedirectToAction("NoPermission");
            }
        }
    }
}