using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;
using Combination0608.ViewModels;
using PagedList;
using System.Net;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Combination0608.Controllers
{
    public class AttMController : Controller
    {
        PCGEntities _db = new PCGEntities();

        private const int PageSize = 10;    //初始設定頁面最多10頁
        int eid = 0;

        private IEnumerable<Zone> ZoneID (int eid)
        {   //地區下拉式選單初始值
            var qq = _db.Permission.Where(x => x.EmployeeID == eid).Select(x => x.ZoneID).FirstOrDefault();
            if (eid != 5)
            {
                return _db.Zone.Where(x => x.ZoneID == qq).OrderBy(x => x.ZoneID);
            }
            else
            {
                return _db.Zone.OrderBy(x => x.ZoneID);
            }
        }

        private IEnumerable<Factories> Factory(int zid,int eid)
        {
            //下拉式選擇之後，會傳回 ZoneID 去搜尋
            if(eid != 5)
            {
                var qq = _db.Permission.Where(x => x.EmployeeID == eid).Select(x => x.FacNo).FirstOrDefault();
                return _db.Factories.Where(x => x.ZoneID == zid && x.FacNo == qq);
            }
            else
            {
                return _db.Factories.Where(x => x.ZoneID == zid);
            }
        }

        private IEnumerable<FacPopulation> Date(string Fac)
        {
            //下拉式選擇之後，會傳回 FacID 去搜尋
            { return _db.FacPopulation.Where(x => x.FacID == Fac); }
        }

        private IEnumerable<AttM> Att
        {   //搜尋結果初始值，把頁面任要列出的、要當關鍵字的任何一筆資料都放進去
            get
            {
                return from x in _db.FacPopulation
                       join y in _db.Factories on x.FacID equals y.FacID
                       join z in _db.Zone on y.ZoneID equals z.ZoneID
                       where x.PopID == 0
                       orderby x.FacID
                       select new AttM { PopID = x.PopID, PopTotal = x.PopTotal, PopNew = x.PopNew, PopLeft = x.PopLeft, FacName = y.FacName, ZoneID = z.ZoneID, Country = z.Country , FacID =x.FacID, Date = x.Date, Date2 = x.Date ,PopLeft3=x.PopLeft3};
            }
        }

        //第一段連動dropdownlist
        public JsonResult FactoryList(int ZoneID,int eid)
        {
            //傳出的值不會顯示所以要一次兩個string，前面值後面內容
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(ZoneID.ToString()))
            {
                var fac = this.GetFac(ZoneID,eid);
                if (fac.Count() > 0)
                {
                    foreach (var f in fac)
                    {
                        //第一個f.facID是你預計要傳出的值，但是他不會顯示，所以後面補第二個參數顯示資料，
                        //add一次只能放一個參數，所以new一個KeyValuePair<string, string>來放兩個參數
                        items.Add(
                            new KeyValuePair<string, string>(
                            f.FacID, string.Format("{0}", f.FacName))
                            );
                    }
                };
            }
            return this.Json(items);
        }

        private IEnumerable<Factories> GetFac(int ZoneID,int eid)
        {
            var qq = _db.Permission.Where(x => x.EmployeeID == eid).Select(x => x.FacNo).FirstOrDefault();
            if(eid != 5)
            {
                var query = _db.Factories.Where(x => x.ZoneID == ZoneID && x.FacNo == qq);
                ViewBag.Factory = query.FirstOrDefault();
                return query.ToList();
            }
            else
            {
                var query = _db.Factories.Where(x => x.ZoneID == ZoneID);
                ViewBag.Factory = query.FirstOrDefault();
                return query.ToList();
            }
        }

        //第二段連動dropdownlist
        public JsonResult DateList(string FacID)
        {
            //傳出的值不會顯示所以要一次兩個string，前面值後面內容
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(FacID))
            {
                var fac = this.GetDate(FacID);
                if (fac.Count() > 0)
                {
                    foreach (var f in fac)
                    {
                        //第一個f.facID是你預計要傳出的值，但是他不會顯示，所以後面補第二個參數顯示資料，
                        //add一次只能放一個參數，所以new一個KeyValuePair<string, string>來放兩個參數
                        items.Add(
                            new KeyValuePair<string, string>(
                            f.Date, string.Format("{0}", f.Date))
                            );
                    }
                };
            }
            return this.Json(items);
        }

        private IEnumerable<FacPopulation> GetDate(string FacID)
        {
            var query = _db.FacPopulation.Where(x => x.FacID == FacID);
            ViewBag.Factory = query.FirstOrDefault();
            return query.ToList();
        }

        // GET: FacPopulations
        [Authorize]
        public ActionResult Index(int page = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                int userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);
                eid = userdata;
                PCGEntities db = new PCGEntities();

                var query = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");
            }

            var query1 = _db.FacPopulation.OrderBy(x => x.FacID);
            var query2 = this.Att;

            int pageIndex = page < 1 ? 1 : page;

            var models = new AttMViewModel
            {
                SearchParaMeter = new AttMSearchViewModel(),
                PageIndex = pageIndex,
                ZoneID = new SelectList(this.ZoneID(eid), "ZoneID", "Country"),
                Factory = new SelectList(this.Factory(0,eid), "FacID", "FacName"),
                Date = new SelectList(this.Date(null), "Date", "Date"),
                Populations = query1.ToPagedList(pageIndex, PageSize),
                AttMSearchList = query2.ToPagedList(pageIndex, PageSize)
            };

            return View(models);
        }

        [HttpPost]
        public ActionResult Index(AttMViewModel model)
        {
            int userdata = 0;
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);
                PCGEntities db = new PCGEntities();

                var query = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");
            }
            eid = Convert.ToInt32(Session["userdata"]);
            var RS = from x in _db.FacPopulation
                     join y in _db.Factories on x.FacID equals y.FacID
                     join z in _db.Zone on y.ZoneID equals z.ZoneID
                     into joined
                     join w in _db.Permission on y.FacNo equals w.FacNo
                     from j in joined.DefaultIfEmpty()
                     orderby j.ZoneID
                     select new AttM { PopID = x.PopID, PopTotal = x.PopTotal, PopNew = x.PopNew, PopLeft = x.PopLeft, Date = x.Date, FacName = y.FacName, ZoneID = j.ZoneID, Country = j.Country, FacID = x.FacID, PopLeft3 = x.PopLeft3, EmployeeID = w.EmployeeID };

            var RS2 = from x in _db.FacPopulation
                     join y in _db.Factories on x.FacID equals y.FacID
                     join z in _db.Zone on y.ZoneID equals z.ZoneID
                     into joined
                     from j in joined.DefaultIfEmpty()
                     orderby j.ZoneID
                     select new AttM { PopID = x.PopID, PopTotal = x.PopTotal, PopNew = x.PopNew, PopLeft = x.PopLeft, Date = x.Date, FacName = y.FacName, ZoneID = j.ZoneID, Country = j.Country, FacID = x.FacID, PopLeft3 = x.PopLeft3};

            if (eid != 5)
            {
                RS.Where(x => x.EmployeeID == eid);
                if (model.SearchParaMeter.ZoneID != 0)
                {
                        RS = RS.Where(x => x.ZoneID == model.SearchParaMeter.ZoneID && x.EmployeeID == eid);
                }
                if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.FacID))
                {
                    RS = RS.Where(x => x.FacID == model.SearchParaMeter.FacID.ToString());
                }
                if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.Date))
                {
                    RS = RS.Where(x => x.Date == model.SearchParaMeter.Date);
                }
            }
            else {
                if (model.SearchParaMeter.ZoneID != 0)
                {
                    RS2 = RS2.Where(x => x.ZoneID == model.SearchParaMeter.ZoneID);
                }
                if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.FacID))
                {
                    RS2 = RS2.Where(x => x.FacID == model.SearchParaMeter.FacID.ToString());
                }
                if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.Date))
                {
                    RS2 = RS2.Where(x => x.Date == model.SearchParaMeter.Date);
                }
            }

            RS = RS.OrderBy(x => x.ZoneID);
            RS2 = RS2.OrderBy(x => x.ZoneID);
            int pageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;

            if (eid != 5) {
                var result = new AttMViewModel
                {
                    SearchParaMeter = model.SearchParaMeter,
                    ZoneID = new SelectList(
                       items: this.ZoneID(eid),
                       dataValueField: "ZoneID",
                       dataTextField: "Country",
                       selectedValue: model.SearchParaMeter.ZoneID),
                    Factory = new SelectList(
                       items: this.Factory(model.SearchParaMeter.ZoneID, eid),
                       dataValueField: "FacID",
                       dataTextField: "FacName",
                       selectedValue: model.SearchParaMeter.FacID),
                    Date = new SelectList(
                   items: this.Date(model.SearchParaMeter.FacID),
                   dataValueField: "Date",
                   dataTextField: "Date",
                   selectedValue: model.SearchParaMeter.Date),
                    PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex,
                    AttMSearchList = RS.ToPagedList(pageIndex, PageSize)
                };
                return View(result);
            }
            else
            {
                var result = new AttMViewModel
                {
                    SearchParaMeter = model.SearchParaMeter,
                    ZoneID = new SelectList(
                       items: this.ZoneID(eid),
                       dataValueField: "ZoneID",
                       dataTextField: "Country",
                       selectedValue: model.SearchParaMeter.ZoneID),
                    Factory = new SelectList(
                       items: this.Factory(model.SearchParaMeter.ZoneID, eid),
                       dataValueField: "FacID",
                       dataTextField: "FacName",
                       selectedValue: model.SearchParaMeter.FacID),
                    Date = new SelectList(
                   items: this.Date(model.SearchParaMeter.FacID),
                   dataValueField: "Date",
                   dataTextField: "Date",
                   selectedValue: model.SearchParaMeter.Date),
                    PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex,
                    AttMSearchList = RS2.ToPagedList(pageIndex, PageSize)
                };
                return View(result);
            }      
        }

        //以下修改
        // GET: FacPopulations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacPopulation facPopulation = _db.FacPopulation.Find(id);
            if (facPopulation == null)
            {
                return HttpNotFound();
            }

            int userdata = 0;
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity idd = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = idd.Ticket;
                userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);
                PCGEntities db = new PCGEntities();

                var query = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");
            }
            eid = Convert.ToInt32(Session["userdata"]);
            var qq = _db.Permission.Where(x => x.EmployeeID == eid).Select(x=>x.FacNo);
            if(eid != 5)
            {
                ViewBag.Factory = new SelectList(_db.Factories.Where(x => x.FacNo == qq.FirstOrDefault()), "FacID", "FacName", facPopulation.FacID);
            }
            else
            {
                ViewBag.Factory = new SelectList(_db.Factories, "FacID", "FacName", facPopulation.FacID);
            }
            return View(facPopulation);
        }

        // POST: FacPopulations/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PopID,Date,FacID,PopTotal,PopNew,PopLeft,PopLeft3")] FacPopulation facPopulation)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(facPopulation).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Factory = new SelectList(_db.Factories, "FacID", "FacName", facPopulation.FacID);
            return View(facPopulation);
        }

        //以下刪除
        // GET: FacPopulations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FacPopulation facPopulation = _db.FacPopulation.Find(id);
            if (facPopulation == null)
            {
                return HttpNotFound();
            }
            return View(facPopulation);
        }

        // POST: FacPopulations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FacPopulation facPopulation = _db.FacPopulation.Find(id);
            _db.FacPopulation.Remove(facPopulation);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        //以下新增// GET: FacPopulations/Create
        public ActionResult Create()
        {
            
            int userdata = 0;
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity idd = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = idd.Ticket;
                userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);
                PCGEntities db = new PCGEntities();

                var query = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");
            }
            eid = Convert.ToInt32(Session["userdata"]);
            var qq = _db.Permission.Where(x => x.EmployeeID == eid).Select(x => x.FacNo);
            if (eid != 5)
            {
                ViewBag.Factory = new SelectList(_db.Factories.Where(x => x.FacNo == qq.FirstOrDefault()), "FacID", "FacName");
            }
            else
            {
                ViewBag.Factory = new SelectList(_db.Factories, "FacID", "FacName");
            }
            return View();
        }

        // POST: FacPopulations/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PopID,Date,FacID,PopTotal,PopNew,PopLeft,PopLeft3")] FacPopulation facPopulation)
        {
            if (ModelState.IsValid)
            {
                _db.FacPopulation.Add(facPopulation);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(facPopulation);
        }
    }
}