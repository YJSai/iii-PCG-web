using Combination0608.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Combination0608.Views
{
    public class ChartController : Controller
    {
        // GET: Chart
        private PCGEntities db = new PCGEntities();
        // GET: CHART
        public ActionResult ReturnData(string FID,string Year)
        {

            var query = (from FP in db.FacPopulation
                         where FP.FacID == FID && FP.Date.Contains(Year)
                         select new
                         {
                             PT = FP.PopTotal,
                             PN = FP.PopNew,
                             PL = FP.PopLeft,
                             PL3 = FP.PopLeft3,
                             Date = FP.Date/*year = Convert.ToDateTime(FP.Date).Year,month = Convert.ToDateTime(FP.Date).Month*/
                         }).ToList();

            //var q = query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PN });
            
            IEnumerable<int[]> arrayA;
            IEnumerable<int[]> arrayB;
            IEnumerable<int[]> arrayC;
            arrayA = query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PN });
            arrayB = query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PT });
            arrayC = query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PL +x.PL3});

            //List<int[]> array = new List<int[]>();
            //foreach (var item in q)
            //{
            //    array.Add(item);
            //}
            //JArray J = new JArray(new JArray());
            //foreach (var item in q)
            //{
            //    JArray JAA = new JArray(item);
            //    J.Add(item);
            //}
            //JArray JA = new JArray(new JObject(
            //           new JProperty("label", "Sea Level Pressure"),
            //           new JProperty("data",new JArray(arrayB)),
            //           new JProperty("color", "#756600"),
            //           new JProperty("bars", new JObject(new JProperty("show", "true"),
            //                              new JProperty("align", "center"),
            //                              new JProperty("barWidth", "24 * 60 * 60 * 600"),
            //                              new JProperty("lineWidth", "1"))


            //           )),
            //           new JObject(
            //           new JProperty("label", "Sea Level Pressure"),
            //           new JProperty("data", new JArray(query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PN }))),
            //           new JProperty("color", "#756600"),
            //           new JProperty("bars", new JObject(new JProperty("show", "true"),
            //                              new JProperty("align", "center"),
            //                              new JProperty("barWidth", "24 * 60 * 60 * 600"),
            //                              new JProperty("lineWidth", "1"))


            //           )),
            //           new JObject(
            //           new JProperty("label", "Sea Level Pressure"),
            //           new JProperty("data", new JArray(query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PN }))),
            //           new JProperty("color", "#756600"),
            //           new JProperty("bars", new JObject(new JProperty("show", "true"),
            //                              new JProperty("align", "center"),
            //                              new JProperty("barWidth", "24 * 60 * 60 * 600"),
            //                              new JProperty("lineWidth", "1"))


            //           ))

            //    );
            //var q = query.Select(x => new int[] { Convert.ToDateTime(x.Date).Month, x.PN });

            //foreach (var item in q)
            //{

            //}
            //JArray JA = new JArray();
            //foreach (var item in q)
            //{
            //    JArray JAA = new JArray(item);
            //    JA.Add(JAA);
            //}


        //    var ret = new[]
        //{
        //    new { label="PageViews", data = query.Select(x=>new int[]{ Convert.ToDateTime(x.Date).Month, x.PN })},
        //    new { label="Visits", data = query.Select(x=>new int[]{ Convert.ToDateTime(x.Date).Month, x.PT })},
        //    new { label="Visitors", data = query.Select(x=>new int[]{ Convert.ToDateTime(x.Date).Month, x.PL })}

        //};
            var back = new[] { arrayA , arrayB ,arrayC };
            return Json(back);
        }

        private IEnumerable<Zone> GetZones()
        {
            var query = db.Zone;
            ViewBag.Factory = query.FirstOrDefault();
            return query.ToList();
        }
        [Authorize]
        public ActionResult Index()
        {

            int userdata = 0;
            if (User.Identity.IsAuthenticated)
            {
                FormsIdentity id = (FormsIdentity)HttpContext.User.Identity;
                FormsAuthenticationTicket ticket = id.Ticket;
                userdata = Convert.ToInt32(ticket.UserData);
                Session.Add("userdata", userdata);
                var query = (from ad in db.Administrators
                             where ad.EmployeeID == userdata
                             select ad.Name).First();
                string name = query;
                Session.Add("Name", name);
            }
            else {
                Session.Add("Name", "Guset");

            }




            SelectList sl = new SelectList(this.GetZones(), "ZoneID", "Country");
            ViewBag.SelectList = sl;
            return View();
            
        }

        public JsonResult year(string FacID) {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
            var query = (from FP in db.FacPopulation
                         where FP.FacID == FacID 
                         select new
                         {
                             Date = FP.Date
                             /*Date = FP.Date*//*year = Convert.ToDateTime(FP.Date).Year,month = Convert.ToDateTime(FP.Date).Month*/
                         });
            foreach (var x in query)
            {
                items.Add(
                    new KeyValuePair<string, string>(
                    Convert.ToDateTime(x.Date).Year.ToString(), Convert.ToDateTime(x.Date).Year.ToString())
                    );
            }
            var distinctDatas = items.Distinct();
            //var qu = query.Select(x => new int[] { Convert.ToDateTime(x.Date).Year });

            return Json(distinctDatas);

        }
    }
}


