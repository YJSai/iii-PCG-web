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
    public class FactoryController : Controller
    {
        private PCGEntities _db = new PCGEntities();

        private const int PageSize = 10;
        int Fac;
        int ZId;

        private IQueryable<AttS> Att1(int? EID,int Zid) {
            if (EID != 5)
            {
                //return from P in _db.Permission 
                //       join F in _db.Factories on P.FacNO equals F.FacNo
                //return from F in _db.Factories
                //       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                //       join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
                //       from j in joined.DefaultIfEmpty() 
                return from P in _db.Permission
                       where (P.EmployeeID == EID)
                       join F in _db.Factories on P.FacNo equals F.FacNo
                       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                       join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
                       from j in joined.DefaultIfEmpty()
                       select new AttS { FacID = F.FacID, Country = Z.Country, EmpName = j.Name, FacName = F.FacName, EditTime = F.EditTime, ZoneID = F.ZoneID, FacNo = F.FacNo/*, EmployeeID = j.EmployeeID */};

            }
            else {
                return from F in _db.Factories
                       where(F.ZoneID == Zid)
                       select new AttS { FacName = F.FacName, FacNo = F.FacNo };
            }


            }




        private IQueryable<AttS> Att(int ? EID)
        {   //搜尋結果初始值
            if (EID != 5)
            {
                //return from P in _db.Permission 
                //       join F in _db.Factories on P.FacNo equals F.FacNo
                //return from F in _db.Factories
                //       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                //       join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
                //       from j in joined.DefaultIfEmpty() 
                return from P in _db.Permission
                       where (P.EmployeeID == EID)
                       join F in _db.Factories on P.FacNo equals F.FacNo
                       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                       join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
                       from j in joined.DefaultIfEmpty()
                       select new AttS { FacID = F.FacID, Country = Z.Country, EmpName = j.Name, FacName = F.FacName, EditTime = F.EditTime, ZoneID = F.ZoneID, FacNo = F.FacNo/*, EmployeeID = j.EmployeeID */};

            }
            else  {
                return
                from F in _db.Factories
                join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
                from j in joined.DefaultIfEmpty()
                orderby Z.ZoneID
                //where Z.ZoneID == ZId && F.FacNo == Fac
                select new AttS { FacID = F.FacID, Country = Z.Country, EmpName = j.Name, FacName = F.FacName, EditTime = F.EditTime, ZoneID = F.ZoneID, FacNo = F.FacNo };
                //return from F in _db.Factories
                //       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                //       join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
                //       from j in joined.DefaultIfEmpty()
                //       orderby Z.ZoneID

                //       select new AttS { FacID = F.FacID, Country = Z.Country, EmpName = j.Name, FacName = F.FacName, EditTime = F.EditTime , ZoneID = F.ZoneID, FacNo = F.FacNo,EmployeeID=j.EmployeeID };
                //return from A in _db.Administrators
                //       join F in _db.Factories on A.FacID equals F.FacID
                //       join Z in _db.Zone on F.ZoneID equals Z.ZoneID
                //       //where Z.ZoneID == ZId && F.FacNo == Fac
                //       orderby Z.ZoneID
                //       //select new { ZID = Z.Country, FacName = F.FacName, Character = A.Character, EmpName = A.Name, EmpEmail = A.Email };
                //       select new AttS { Country = Z.Country, FacName = F.FacName, Character = A.Character, EmpName = A.Name, EmpEmail = A.Email }; ;
            }
        }

        private IEnumerable<Factories> Factory(int? ZoneId)
        {


            /*get*/
            { return _db.Factories.Where(x => x.ZoneID == ZoneId); }

        }

        //new List<Factories>();
        //
        private IEnumerable<Zone> ZoneID (int sID)
        {
            //get {}
            //if (sID !=5)
            //{
            //    return _db.Permission.GroupBy(x => x.EmployeeID).Select(g => g.First()).OrderBy(y =>y.ZoneID);
            //}

            //from Z in _db.Zone
            //join P in _db.Permission

            if (sID !=5)
            {
                var tt = (from P in _db.Permission
                          where (P.EmployeeID == sID)

                          select P.ZoneID).FirstOrDefault();

                return _db.Zone.Where(x => x.ZoneID == tt).OrderBy(y => y.ZoneID);
                //return tt.OrderBy(x => x.ZoneID);
            }
            


             return _db.Zone.OrderBy(x => x.ZoneID); 
        }

        private Dictionary<string, string> GetFactoryByCity(int ZoneID)
        {
            int empID;

            string xxx = Session["EmployeeID"].ToString();
            int.TryParse(xxx, out empID);
            if (empID != 5)
            {
                var query = this.Att(empID).OrderBy(x => x.FacNo);
                return query.ToDictionary(x => x.FacNo.ToString(), x => x.FacName);
            }
            else {
                var query = _db.Factories
                        .Where(x => x.ZoneID == ZoneID)
                        .Select(
                            x => new
                            {
                                FacNo = x.FacNo,
                                FacName = x.FacName
                            })
                        .OrderBy(x => x.FacNo);
                return query.ToDictionary(x => x.FacNo.ToString(), x => x.FacName);
            }
            //var query = _db.Factories
            //              .Where(x => x.ZoneID == ZoneID)
            //              .Select(
            //                  x => new
            //                  {
            //                      FacID = x.FacNo,
            //                      FacName = x.FacName
            //                  })
            //              .OrderBy(x => x.FacID);

            //return query.ToDictionary(x => x.FacID.ToString(), x => x.FacName);
        }
        // GET: Product
        [Authorize]
        public ActionResult Index(int page = 1)
        {

            //登入時取得session empolyeeID
        
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                int userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("EmployeeID", userdata);

                PCGEntities db = new PCGEntities();

                var queryname = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = queryname;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");

            }








            int empID;
            string xxx ="";
            if (Session["EmployeeID"] != null)
            {
                xxx = Session["EmployeeID"].ToString();
            }
             
                int.TryParse(xxx ,out empID);
                
            
            
            //int empID = Convert.ToInt32(Session["EmployeeID"].ToString());
            //.Where(y=> y.EmployeeID == empID);
            var querry = this.Att(empID).OrderBy(x => x.ZoneID);
            var query = _db.Factories.OrderBy(x => x.ZoneID);
            //var qry = from gu in _db.Factories
            //          from cu in _db.Zone
            //          where gu.ZoneID == cu.ZoneID
            //          select new { cu, gu };
            int pageIndex = page < 1 ? 1 : page;

            var model = new FactoryListViewModel
            {
                SearchParaMeter = new FactorySearchViewModel(),
                PageIndex = pageIndex,
                Factory = new SelectList(this.Factory(null), "FacNo", "FacName"),
                ZoneID = new SelectList(this.ZoneID(empID), "ZoneID", "Country"),
                廠別s = query.ToPagedList(pageIndex, PageSize),
                AttSearchList = querry.ToPagedList(pageIndex, PageSize)
            };

            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Factories factories = _db.Factories.Find(id);
            //if (factories == null)
            //{
            //    return HttpNotFound();
            //}
           
                //ViewBag.Supervisor = new SelectList(_db.Administrators, "EmployeeID", "Name", factories.Supervisor);
                //ViewBag.ZoneID = new SelectList(_db.Zone, "ZoneID", "Country", factories.ZoneID);
            
           

            
             return View(model); 

            
        }

        public JsonResult FactoryName(string CityID)
        {

            var items = new List<KeyValuePair<string, string>>();
            //int empID = Convert.ToInt32(Session["EmployeeID"].ToString());
            //產品&& empID ==5
            int categoryId = 0;
            if (int.TryParse(CityID, out categoryId) )
            {
                var couties = this.GetFactoryByCity(categoryId);
                foreach (var county in couties)
                {
                    items.Add(new KeyValuePair<string, string>(county.Key, county.Value));
                }
            }

            return this.Json(items);
        }


        [HttpPost]

        public ActionResult Index(FactoryListViewModel model)

        {

            //var query = _db.Factories.AsQueryable();
            int empID = Convert.ToInt32(Session["EmployeeID"].ToString());
            var RS =this.Att(empID);
            //var RSS = from F in _db.Factories
            //         join Z in _db.Zone on F.ZoneID equals Z.ZoneID
            //         join A in _db.Administrators on F.Supervisor equals A.EmployeeID into joined
            //         from j in joined.DefaultIfEmpty()
            //         orderby Z.ZoneID
            //         //where Z.ZoneID == ZId && F.FacNo == Fac
            //         select new AttS { FacID = F.FacID, Country = Z.Country, EmpName = j.Name, FacName = F.FacName, EditTime = F.EditTime, ZoneID = F.ZoneID, FacNo = F.FacNo };

            if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.FactoryName))

            {
                RS = RS.Where(x => x.FacName.Contains(model.SearchParaMeter.FactoryName)) ;
                //query = query.Where(

                //    x => x.FacName.Contains(model.SearchParaMeter.FactoryName));

            }

            if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.FactoryID))

            {


                RS = RS.Where(

                        x => x.FacID.Contains(model.SearchParaMeter.FactoryID));
                //query = query.Where(

                //        x => x.FacID.Contains(model.SearchParaMeter.FactoryID));

            }


            if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.ZoneID)
                &&
                int.TryParse(model.SearchParaMeter.ZoneID, out ZId))
            {
                RS = RS.Where(x => x.ZoneID == ZId);
                //query = query.Where(x => x.ZoneID == ZId);
            }



            if (!string.IsNullOrWhiteSpace(model.SearchParaMeter.Factory)
                &&
                int.TryParse(model.SearchParaMeter.Factory, out Fac))
            {   /*model.search = Fac;*/
                RS = RS.Where(x => x.FacNo == Fac);
                //query = query.Where(x => x.FacNo == Fac);
            }
            //query = query.OrderBy(x => x.ZoneID);
            RS = RS.OrderBy(x => x.ZoneID);
            int pageIndex = model.PageIndex < 1 ? 1 : model.PageIndex;


            var result = new FactoryListViewModel

            {
                AttSearchList = RS.ToPagedList(pageIndex, PageSize),
                SearchParaMeter = model.SearchParaMeter,
                ZoneID = new SelectList(
                        items: this.ZoneID(empID), dataValueField: "ZoneID",
                        dataTextField: "Country",
                        selectedValue: model.SearchParaMeter.ZoneID),
                Factory = new SelectList(
                        items: this.Att1(empID,ZId), dataValueField: "FacNo",
                        dataTextField: "FacName",
                        selectedValue: model.SearchParaMeter.Factory),
                PageIndex = model.PageIndex < 1 ? 1 : model.PageIndex,


                //廠別s = query.ToPagedList(pageIndex, PageSize),
                

            };


            //if (id != null)
            //{
            //    Factories factories = _db.Factories.Find(id);
            //    ViewBag.Supervisor = new SelectList(_db.Administrators, "EmployeeID", "Name", factories.Supervisor);
            //    ViewBag.ZoneID = new SelectList(_db.Zone, "ZoneID", "Country", factories.ZoneID);
            //    return PartialView("_EditPartial", factories);
            //}

            //else { return View(result); }
            return View(result);
        }

        // GET: Factories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factories factories = _db.Factories.Find(id);
            if (factories == null)
            {
                return HttpNotFound();
            }
            ViewBag.Supervisor = new SelectList(_db.Administrators, "EmployeeID", "Name", factories.Supervisor);
            ViewBag.ZoneID = new SelectList(_db.Zone, "ZoneID", "Country", factories.ZoneID);
            //return PartialView("_EditPartial",factories);
            return View(factories);
        }

        // POST: Factories/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FacNo,FacID,FacName,ZoneID,EditTime,Supervisor")] Factories factories)
        {
            if (ModelState.IsValid)
            {
                string dateString = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
                string pattern = "yyyy/MM/dd HH:mm";
                DateTime parsedDate ;

                if (DateTime.TryParseExact(dateString, pattern, null, DateTimeStyles.None, out parsedDate))
                {
                    factories.EditTime = parsedDate;
                }
                _db.Entry(factories).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Supervisor = new SelectList(_db.Administrators, "EmployeeID", "Name", factories.Supervisor);
            ViewBag.ZoneID = new SelectList(_db.Zone, "ZoneID", "Country", factories.ZoneID);
            
            return View(factories);
        }

        // GET: Factories/Create
        public ActionResult Create()
        {
            ViewBag.Supervisor = new SelectList(_db.Administrators, "EmployeeID", "Name");
            ViewBag.ZoneID = new SelectList(_db.Zone, "ZoneID", "Country");
            
            return View();
        }

        // POST: Factories/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FacNo,FacID,FacName,ZoneID,EditTime,Supervisor")] Factories factories)
        {
            if (ModelState.IsValid)
            {
                _db.Factories.Add(factories);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Supervisor = new SelectList(_db.Administrators, "EmployeeID", "Name", factories.Supervisor);
            ViewBag.ZoneID = new SelectList(_db.Zone, "ZoneID", "Country", factories.ZoneID);
        
            return View(factories);
        }

        //以下刪除
        // GET: FacPopulations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Factories fac = _db.Factories.Find(id);
            if (fac == null)
            {
                return HttpNotFound();
            }
            return View(fac);
        }

        // POST: FacPopulations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Factories fac = _db.Factories.Find(id);
            _db.Factories.Remove(fac);
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

    }
}