using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;
using System.Web.Security;

namespace Combination0608.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            var temp = ViewData["error"];
            ViewData["error"] = temp;

            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                int userdata = Convert.ToInt32(ticket.UserData);
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
            


            return View();
        }
        [HttpPost]
        public ActionResult Login(string Account, string Password)
        {

            
            PCGEntities db = new PCGEntities();
            List<character> model = new List<character>();
            var query = from acc in db.Administrators
                        where acc.Account == Account
                        select new character
                        {
                            Account = acc.Account,
                            Password = acc.Password,
                           EmployeeID=acc.EmployeeID,
                           Name=acc.Name
                           
                        };
            foreach (var item in query)
                model.Add(new character
                {
                    Account = item.Account,
                    Password = item.Password,
                    EmployeeID = item.EmployeeID,
                    Name=item.Name

                });
            if (model.Count != 0)
            {
                var selectpassword = model.Select(x => x.Password).First().ToString();
                var selectaccount = model.Select(x => x.Account).First().ToString();
                var selectemployeeID = model.Select(x => x.EmployeeID).First().ToString();
                var selname = model.Select(x => x.Name.First()).ToString();
                if (selectpassword.ToString() == Password.ToString() && selectaccount == Account)
                {
                    
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        Account,
                        DateTime.Now,
                        DateTime.Now.AddMinutes(30),
                        true, //是否存入session
                        selectemployeeID,
                        FormsAuthentication.FormsCookiePath
                        );
                   

                    string enticket = FormsAuthentication.Encrypt(ticket); //加密
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, enticket);//新建一個cookie
                    string empID = ticket.UserData;
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    //ViewData["userdata"] = ticket.UserData;
                    Session.Add("EmployeeID", ticket.UserData);
                    Session["EmployeeName"]= ticket.Name;
                    Session.Timeout = 20;
                    string returnUrl = "~"+Session["ReturnUrl"].ToString();
                    return Redirect(returnUrl);
                }
            }
            ViewData["error"] = "無此帳號或密碼有誤";
            return View();
        }
      
        public ActionResult Logout()
        {
            //登出，清除cookie
            FormsAuthentication.SignOut();
            
            Session.Clear();
            //Session.Add("EmployeeName","Guest");
            Session.Add("ReturnUrl", @"/Login/Login");
            return RedirectToAction("Login");
            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            //HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            //cookie2.Expires = DateTime.Now.AddYears(-1);
            //Response.Cookies.Add(cookie2);
            //HttpCookie cookie3 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            //cookie3.Expires = DateTime.Now.AddYears(-1);
            //Response.Cookies.Add(cookie3);
            //FormsAuthentication.RedirectToLoginPage();

        }
        public class character
        {

            public string Account { set; get; }
            public string Password { set; get; }
            public int EmployeeID { set; get; }
            public string Name { set; get; }
        }
    }
}