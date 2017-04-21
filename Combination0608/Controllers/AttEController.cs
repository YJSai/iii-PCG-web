using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using testExcel.Infrastructure.CustomResults;
using System.Web.Security;

namespace Combination0608.Controllers
{
    public class AttEController : Controller
    {
        PCGEntities _db = new PCGEntities();
        // 第一個Dropdownlist
        [Authorize]
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

        private IEnumerable<Zone> GetZones()
        {
            var query = _db.Zone;
            ViewBag.Factory = query.FirstOrDefault();
            return query.ToList();
        }

        //選完第一個後要POST回第一個可選的ZoneID
        [HttpPost]
        public JsonResult Facs(string ZoneID)
        {
            //傳出的值不會顯示所以要一次兩個string，前面值後面內容
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(ZoneID))
            {
                var fac = this.GetFacs(ZoneID);
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

        private IEnumerable<Factories> GetFacs(string ZoneID)
        {
            //廠別裡的ZoneID和選到的ZoneID相同
            var query = _db.Factories.Where(x => x.ZoneID.ToString() == ZoneID);
            return query.ToList();
        }

        //選完第二個後要POST回第三個可選的FacID
        [HttpPost]
        public JsonResult Month1(string FacID)
        {
            //傳出的值不會顯示所以要一次兩個string，前面值後面內容
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(FacID))
            {
                var fac = this.GetMon1(FacID);
                if (fac.Count() > 0)
                {
                    foreach (var f in fac)
                    {
                        items.Add(
                            new KeyValuePair<string, string>(
                            f.Date, string.Format("{0}", f.Date))
                            );
                    }
                };
            }
            return this.Json(items);
        }

        private IEnumerable<FacPopulation> GetMon1(string FacID)
        {
            //人數裡的FacID和選到的FacID相同
            var query = _db.FacPopulation.Where(x => x.FacID == FacID);
            return query.ToList();
        }

        //選完第三個後要POST回第四個可選的Date
        [HttpPost]
        public JsonResult Month2(string Date, string FacID)
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(Date))
            {
                var fac = this.GetMon2(Date, FacID);
                if (fac.Count() > 0)
                {
                    foreach (var f in fac)
                    {
                        items.Add(
                            new KeyValuePair<string, string>(
                            f.Date, string.Format("{0}", f.Date))
                            );
                    }
                };
            }
            return this.Json(items);
        }

        private IEnumerable<FacPopulation> GetMon2(string Date, string FacID)
        {
            var query = _db.FacPopulation.Where(x => x.Date.CompareTo(Date) >= 0 && x.FacID == FacID);
            return query.ToList();
        }

        //列表
        [HttpPost]
        public JsonResult Lists(string Date1,
string Date2, string FacID,int Country)
        {
            var qq = _db.FacPopulation.Join(
                _db.Factories,
                x => x.FacID,
                y => y.FacID,
                (x, y) => new
                {
                    FacID = x.FacID,
                    Date = x.Date,
                    PopTotal = x.PopTotal,
                    PopNew = x.PopNew,
                    PopLeft = x.PopLeft,
                    FacName = y.FacName,
                    ZoneID = y.ZoneID,
                    PopLeft3 = x.PopLeft3
                });
            if ((Date1.Trim()).Length > 0 && (Date2.Trim()).Length > 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.Date.CompareTo(Date1) >= 0 && x.Date.CompareTo(Date2) <= 0 && x.FacID == FacID);
            }
            if ((Date1.Trim()).Length > 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.Date.CompareTo(Date1) >= 0 && x.FacID == FacID);
            }
            if ((Date1.Trim()).Length == 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.FacID == FacID);
            }
            if ((Date1.Trim()).Length == 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length == 0 && Country > 0)
            {
                qq = qq.Where(x => x.ZoneID == Country);
            }

            return Json(qq.ToList());
        }


        //檢查有無資料
        [HttpPost]
        public ActionResult HasData(string Date1, string Date2, string FacID, int Country)
        {
            JObject jo = new JObject();
            var qq = _db.FacPopulation.Join(
                _db.Factories,
                x => x.FacID,
                y => y.FacID,
                (x, y) => new
                {
                    FacID = x.FacID,
                    Date = x.Date,
                    PopTotal = x.PopTotal,
                    PopNew = x.PopNew,
                    PopLeft = x.PopLeft,
                    FacName = y.FacName,
                    ZoneID = y.ZoneID,
                    PopLeft3 = x.PopLeft3
                });
            if ((Date1.Trim()).Length > 0 && (Date2.Trim()).Length > 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.Date.CompareTo(Date1) >= 0 && x.Date.CompareTo(Date2) <= 0 && x.FacID == FacID);
            }
            if ((Date1.Trim()).Length > 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.Date.CompareTo(Date1) >= 0 && x.FacID == FacID);
            }
            if ((Date1.Trim()).Length == 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.FacID == FacID);
            }
            if ((Date1.Trim()).Length == 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length == 0 && Country > 0)
            {
                qq = qq.Where(x => x.ZoneID == Country);
            }

            bool result = !qq.Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }

        //匯出
        public ActionResult Export(string Date1, string Date2, string FacID,int Country)
        {
            var exportSpource = this.GetExportDataWithAllColumns(Date1, Date2, FacID,Country);
            var dt = JsonConvert.DeserializeObject<System.Data.DataTable>(exportSpource.ToString());

            var exportFileName =
                string.Concat("FactoryPopulation_", DateTime.Now.ToString("yyyyMMddHHmm"), ".xlsx");

            return new ExportExcelResult
            {
                SheetName = "工廠人數",
                FileName = exportFileName,
                ExportData = dt
            };
        }

        private JArray GetExportDataWithAllColumns(string Date1, string Date2, string FacID,int Country)
        {
            var qq = _db.FacPopulation.Join(
                _db.Factories,
                x => x.FacID,
                y => y.FacID,
                (x, y) => new
                {
                    FacID = x.FacID,
                    Date = x.Date,
                    PopTotal = x.PopTotal,
                    PopNew = x.PopNew,
                    PopLeft = x.PopLeft,
                    FacName = y.FacName,
                    ZoneID = y.ZoneID,
                    PopLeft3 = x.PopLeft3
                });
            if ((Date1.Trim()).Length > 0 && (Date2.Trim()).Length > 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.Date.CompareTo(Date1) >= 0 && x.Date.CompareTo(Date2) <= 0 && x.FacID == FacID);
            }
            if ((Date1.Trim()).Length > 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.Date.CompareTo(Date1) >= 0 && x.FacID == FacID);
            }
            if ((Date1.Trim()).Length == 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length > 0 && Country > 0)
            {
                qq = qq.Where(x => x.FacID == FacID);
            }
            if ((Date1.Trim()).Length == 0 && (Date2.Trim()).Length == 0 && (FacID.Trim()).Length == 0 && Country > 0)
            {
                qq = qq.Where(x => x.ZoneID == Country);
            }

            JArray jObjects = new JArray();

            foreach (var item in qq)
            {
                double x1 = Convert.ToDouble(item.PopTotal.ToString());
                double x2 = Convert.ToDouble(item.PopLeft.ToString());
                double x3 = Convert.ToDouble(item.PopLeft3.ToString());
                string r = string.Format("{0:P}", ((x2+ x3) / x1));
                var jo = new JObject
                {
                    {"廠別", item.FacName},
                    {"日期", item.Date},
                    {"月底人數", item.PopTotal},
                    {"新進人員", item.PopNew},
                    {"離職人員（正式員工）", item.PopLeft},
                    {"離職人員（試用期）", item.PopLeft3},
                    {"離職率",r}
                };
                jObjects.Add(jo);
            }
            return jObjects;
        }


    }
}