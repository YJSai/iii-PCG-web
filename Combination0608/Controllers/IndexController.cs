using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;
using System.Net;
using PagedList;
using System.Web.Security;
using Newtonsoft.Json.Linq;

namespace Combination0608.Controllers
{

    public class IndexController : Controller
    {
        private PCGEntities _db = new PCGEntities();

        // GET: Announcements
        public ActionResult Index()
        {

            int userdata = 0;
            if (User.Identity.IsAuthenticated)//姓名SESSION驗證
            {
                PCGEntities db = new PCGEntities();
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);
                var query3 = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query3;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");

            }




            var query = _db.Announcement.OrderByDescending(x => x.ancID).Take(10);
 
            return View(query.ToList()); 
        }

        [HttpPost]
        public ActionResult Index(Announcement ann)
        {
            var query = _db.Announcement.OrderByDescending(x => x.ancID).Take(10);

            return View(query.ToList());
        }

        public ActionResult List()
        {
            var query = _db.Announcement.OrderByDescending(x => x.ancID);
            return View(query.ToList());
        }

        // GET: Announcements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Announcement announcement = _db.Announcement.Find(id);
            if (announcement == null)
            {
                return HttpNotFound();
            }
            return View(announcement);
        }

        // GET: Announcements/Create
        [Authorize]
        public ActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                int userdata = Convert.ToInt32(ticket.UserData.ToString());
                var qq = _db.Administrators.Where(x => x.EmployeeID == userdata).Select(x=>new {
                    name = x.Name
                }).FirstOrDefault();
                Session.Add("userdata", qq.name.ToString());
                PCGEntities db = new PCGEntities();

                var query = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query;
                Session.Add("Name", name);


            }
            return View();
        }

        // POST: Announcements/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ancID,publisher,ancDate,ancTitle,ancContent")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                _db.Announcement.Add(announcement);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(announcement);
        }
    }
}