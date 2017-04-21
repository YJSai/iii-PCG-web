using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;
using System.Data.Entity;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;

using System.Net;
using System.Net.Http;
using System.Data.SqlClient;

namespace Combination0608.Controllers
{
    public class PMCController : Controller
    {
        // GET: PMC
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult PMSTART()
        {// 取得現在的使用者
            int userdata = 0;
            PCGEntities db = new PCGEntities();
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

            int PromissionID = Convert.ToInt32(userdata);//取得員工ID
            if (PromissionID == 5)
            {//如果是superuser

                List<PersonInformation> model = new List<PersonInformation>();
                var query = from z in db.Zone//不限定條件
                            select new PersonInformation
                            {
                                ZoneID = z.ZoneID,
                                Country = z.Country
                            };
                foreach (var item in query) //retrieve each item and assign to model
                {
                    model.Add(new PersonInformation()
                    {
                        ZoneID = item.ZoneID,
                        Country = item.Country
                    });
                }
                return View(model);

            }
            else {//其他使用者
                List<PersonInformation> model = new List<PersonInformation>();
                var query = from z in db.Zone//根據地區限定條件
                            join per in db.Permission
                            on z.ZoneID equals per.ZoneID
                            where per.ZoneID ==z.ZoneID && per.EmployeeID== userdata
                            select new PersonInformation
                            {
                                ZoneID = z.ZoneID,
                                Country = z.Country
                            };
                query = query.Distinct();

                foreach (var item in query) //retrieve each item and assign to model
                {
                    model.Add(new PersonInformation()
                    {
                        ZoneID = item.ZoneID,
                        Country = item.Country
                    });
                }
                return View(model);

            }
          

            //var query = from z in db.Zone
            //            join Fac in db.Factories
            //            on z.ZoneID equals Fac.ZoneID
            //            join per in db.Permission
            //            on z.ZoneID equals per.ZoneID
            //            where per.EmployeeID == PromissionID
            //            select new PersonInformation
            //            {
            //                ZoneID = z.ZoneID,
            //                Country = z.Country
            //            };
            //query = query.GroupBy(x =>x.FacName).Select(x => x.First());

            
        }

        public class PersonInformation
        {
            //----------------管理人員----------------
            public int EmployeeID { get; set; }
            public string FacID { get; set; }
            public string Name { get; set; }
            public string Character { get; set; }
            public string Email { get; set; }
            public string Account { get; set; }
            public string Password { get; set; }
            //------------------廠別---------------
            public int FacNo { get; set; }
            public string FacName { get; set; }
            public Nullable<int> ZoneID { get; set; }
            //--------------權限-------------
            public int PermissionID { get; set; }
            //-----------地區-----------
            public string Country { get; set; }

            //-------------------------------------
            public IEnumerable<string> Contacts { get; set; }



            //-----------------------------------
        }
        [HttpPost]
        public ActionResult PM(int ZoneID)
        {
            PCGEntities db = new PCGEntities();
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

            ZoneID = Convert.ToInt32(Request["ZoneID"]);
            //挑出地區名稱後做廠別篩選
            List<PersonInformation> model = new List<PersonInformation>();

            if (userdata != 5)//不是管理者
            {
                var query = from per in db.Permission
                            join f in db.Factories
                            on per.FacNo equals f.FacNo
                            where per.ZoneID==ZoneID
                            join ad in db.Administrators
                            on f.FacID equals ad.FacID
                            where ad.EmployeeID== userdata
                            select new PersonInformation
                            {
                                ZoneID = f.ZoneID,
                                FacName = f.FacName,
                                FacNo = f.FacNo,
                                Character = ad.Character,
                                EmployeeID = ad.EmployeeID,
                                Name = ad.Name
                            };
                query=query.Distinct();
                foreach (var item in query) //retrieve each item and assign to model
                {
                    model.Add(new PersonInformation()
                    {
                        ZoneID = item.ZoneID,
                        FacName = item.FacName,
                        FacNo = item.FacNo,
                        Character = item.Character,
                        EmployeeID = item.EmployeeID,
                        Name = item.Name
                        //FacNo用來作為 地區下拉式選單的value廠別的下拉式選單篩選條件，千萬別刪掉
                    });
                }
                return Json(new { model });
            }
            else { //管理者
                var query = from f in db.Factories
                            join ad in db.Administrators
                            on f.FacID equals ad.FacID
                            where f.ZoneID == ZoneID
                            orderby ad.EmployeeID
                            select new PersonInformation
                            {
                                ZoneID = f.ZoneID,
                                FacName = f.FacName,
                                FacNo = f.FacNo,
                                Character = ad.Character,
                                EmployeeID = ad.EmployeeID,
                                Name = ad.Name
                            };
               
                foreach (var item in query) //retrieve each item and assign to model
                {
                    model.Add(new PersonInformation()
                    {
                        ZoneID = item.ZoneID,
                        FacName = item.FacName,
                        FacNo = item.FacNo,
                        Character = item.Character,
                        EmployeeID = item.EmployeeID,
                        Name = item.Name
                        //FacNo用來作為 地區下拉式選單的value廠別的下拉式選單篩選條件，千萬別刪掉
                    });
                }
                return Json(new { model });

            }

            
        }
        [HttpPost]
        public ActionResult PM2(int FacNo)
        {//回來記得弄登入者全縣
            //FacNo = Convert.ToInt32(Request["FacNo"]);
            //挑出地區名稱後做廠別篩選
            PCGEntities db = new PCGEntities();
            List<PersonInformation> model = new List<PersonInformation>();
            var query = from f in db.Factories
                        join ad in db.Administrators
                        on f.FacID equals ad.FacID
                        where f.FacNo == FacNo
                        orderby ad.EmployeeID
                        select new PersonInformation
                        {
                            FacName = f.FacName,
                            Character = ad.Character,
                            EmployeeID = ad.EmployeeID,
                            Name = ad.Name
                        };
            foreach (var item in query) //retrieve each item and assign to model
            {
                model.Add(new PersonInformation()
                {
                    FacName = item.FacName,
                    EmployeeID = item.EmployeeID,
                    Character = item.Character,
                    Name = item.Name
                });
            }
            return Json(new { model });
        }




        //-----------------------管理人員資料新增-----------------------------

        public ActionResult ADDAD()
        {
            PCGEntities db = new PCGEntities();
            PersonInformation ad = new PersonInformation();
            List<PersonInformation> model = new List<PersonInformation>();

            var query = from f in db.Factories
                        join z in db.Zone
                        on f.ZoneID equals z.ZoneID
                        select new PersonInformation
                        {
                            FacID = f.FacID,
                            FacName = f.FacName,
                            FacNo = f.FacNo,
                            ZoneID = f.ZoneID,
                            Country = z.Country

                        };
            foreach (var item in query) {
                model.Add(new PersonInformation
                {
                    FacID = item.FacID,
                    FacName = item.FacName,
                    FacNo = item.FacNo,
                    ZoneID = item.ZoneID,
                    Country = item.Country

                });

            }

            return View(model);
        }
        [HttpPost]
        public ActionResult ADDAD(PersonInformation ad,FormCollection chk)
        {
            TempData["Name"] = ad.Name;
            TempData["Character"] = ad.Character;
            TempData["Email"] = ad.Email;
            TempData["Account"] = ad.Account;
            TempData["Password"] = ad.Password;
            if (string.IsNullOrEmpty(ad.Name))
            {
                this.ModelState.AddModelError("Name", "Nmae不可為空白");
                TempData["Nameerror"] = "Name不可為空白";
            }
            if (string.IsNullOrEmpty(ad.Character))
            {
                this.ModelState.AddModelError("Charactererror", "Character不可為空白");
                TempData["Charactererror"] = "Character不可為空白";

            }
            if (string.IsNullOrEmpty(ad.Email))
            {
                this.ModelState.AddModelError("Emailerror", "Email不可為空白");
                TempData["Emailerror"] = "Email不可為空白";
            }
            if (string.IsNullOrEmpty(ad.Account))
            {
                this.ModelState.AddModelError("Account", "Account不可為空白");
                TempData["Accounterror"] = "Account不可為空白";
            }
            if (string.IsNullOrEmpty(ad.Password))
            {
                this.ModelState.AddModelError("Password", "Password不可為空白");
                TempData["Passworderror"] = "Password不可為空白";
            }
            if (ModelState.IsValid)
            {
                PCGEntities db = new PCGEntities();
                Administrators ad2 = new Administrators();
                ad2.FacID = ad.FacID;
                ad2.Character = ad.Character;
                ad2.Name = ad.Name;
                ad2.Email = ad.Email;
                ad2.Account = ad.Account;
                ad2.Password = ad.Password;
                db.Entry(ad2).State = EntityState.Added;
                db.SaveChanges();
                string[] values = chk.GetValues("HaveSelect");//取得要的權限的工廠名字
                for (var i = 0; i < values.Count(); i++)
                {
                    int FacNo = Convert.ToInt32(values[i]);
                    var queryzoneID = from f in db.Factories //查出挑出一項的zoneID
                                      where f.FacNo == FacNo
                                      select f.ZoneID;
                    var queryFacNo = from f in db.Factories //查出挑出一項的zoneID
                                     where f.FacNo == FacNo
                                     select f.FacNo;
                    var queryemp = (from em in db.Administrators
                                    orderby em.EmployeeID descending
                                    select em.EmployeeID).Take(1);//查出現在最高值
                    var maxstringemp = queryemp;//取得新增之後的emp名
                    int MaxIntEmp = maxstringemp.First();
                    int IntFacNo = queryFacNo.First();
                    int IntZone = (int)queryzoneID.First();
                    Permission per = new Permission();
                    per.EmployeeID = MaxIntEmp;
                    per.ZoneID = IntZone;
                    per.FacNo = IntFacNo;
                    db.Entry(per).State = EntityState.Added;
                    db.SaveChanges();
                }
                TempData["Name"]=null;
                TempData["Character"] = null;
                TempData["Email"] = null;
                TempData["Account"] = null;
                TempData["Password"] = null;
                return RedirectToAction("PMSTART");
            }
            return Redirect("ADDAD");
        }




        //-----------------------管理人員資料新增-----------------------------
        //-----------------------管理人員資料修改-----------------------------

        public ActionResult Edit(string btnNUM)
        {
            btnNUM = Request.QueryString["EMID"];
            int EmployeeID = Convert.ToInt32(btnNUM.Remove(0, 9));
            //int EmployeeID = 44;
            //int EmployeeID = 5;//debug用
            PCGEntities db = new PCGEntities();
            List<PersonInformation> model = new List<PersonInformation>();
            var query = from z in db.Administrators
                        where z.EmployeeID == EmployeeID
                        select new PersonInformation
                        {
                            EmployeeID = z.EmployeeID,
                            FacID = z.FacID,
                            Name = z.Name,
                            Character = z.Character,
                            Email = z.Email,
                            Account = z.Account,
                            Password = z.Password

                        };
            foreach (var item in query) //retrieve each item and assign to model
            {
                model.Add(new PersonInformation()
                {
                    EmployeeID = item.EmployeeID,
                    FacID = item.FacID,
                    Name = item.Name,
                    Character = item.Character,
                    Email = item.Email,
                    Account = item.Account,
                    Password = item.Password
                });

                //string btnNUM = Request.QueryString["EMID"];
                //int EmployeeID = Convert.ToInt32(btnNUM.Remove(0, 9));
                //PCGEntities db = new PCGEntities();
                //Administrators ad = db.Administrators.Find(EmployeeID);
                //return View(ad);
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(PersonInformation edit)
        {
            PCGEntities db = new PCGEntities();

            Administrators ad2 = db.Administrators.Find(edit.EmployeeID);
            ad2.FacID = edit.FacID;
            ad2.Character = edit.Character;
            ad2.Name = edit.Name;
            ad2.Email = edit.Email;
            ad2.Account = edit.Account;
            ad2.Password = edit.Password;

            db.Entry(ad2).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Edit");
            //PCGEntities db = new PCGEntities();
            //db.Entry(ad).State = EntityState.Modified;
            //db.SaveChanges();
            //return View(ad);
        }
        [HttpPost]
        public ActionResult EditHavepermission(int employeeID)
        {
            PCGEntities db = new PCGEntities();
            List<PersonInformation> model = new List<PersonInformation>();
            var query = from per in db.Permission
                        where per.EmployeeID == employeeID
                        join f in db.Factories
                        on per.FacNo equals f.FacNo
                        join z in db.Zone
                        on per.ZoneID equals z.ZoneID 
                        select new PersonInformation {
                            ZoneID=z.ZoneID,
                            FacName =f.FacName,
                            Country=z.Country,
                            FacNo = f.FacNo
                        };
            foreach (var item in query) {
                model.Add(new PersonInformation
                {
                    ZoneID = item.ZoneID,
                    FacName = item.FacName,
                    Country = item.Country,
                    FacNo = item.FacNo
                });

            }
            return Json(model);
        }
        [HttpPost]
        public ActionResult EditNonHavepermission(int employeeID)
        {
            PCGEntities db = new PCGEntities();
            var queryselzoneID = from z in db.Zone// 先找出該員在哪一地區
                                 join f in db.Factories
                                 on z.ZoneID equals f.ZoneID
                                 join ad in db.Administrators
                                 on f.FacID equals ad.FacID
                                 where ad.EmployeeID == employeeID
                                 select z;
           int zoneID = queryselzoneID.Select(x => x.ZoneID).First();

            List < PersonInformation > model = new List<PersonInformation>();
            var query = from f in db.Factories
                        join z in db.Zone
                        on f.ZoneID equals z.ZoneID
                        join ad in db.Administrators
                        on f.FacID equals ad.FacID
                        where  f.ZoneID == zoneID
                        join per in db.Permission
                        on ad.EmployeeID equals per.EmployeeID
                        //where per.EmployeeID!=employeeID//去掉權限
                        select new PersonInformation
                        {
                            ZoneID = z.ZoneID,
                            FacName = f.FacName,
                            Country = z.Country,
                            FacNo=f.FacNo
                        };
            query = query.Distinct();
            foreach (var item in query)
            {
                model.Add(new PersonInformation
                {
                    ZoneID = item.ZoneID,
                    FacName = item.FacName,
                    Country = item.Country,
                    FacNo = item.FacNo
                });

            }
            return Json(model);
        }




        //-----------------------管理人員資料修改-----------------------------


        //-----------------------管理人員資料刪除-----------------------------
        public ActionResult Delete()
        {
            //string btnNUM = Request.QueryString["EMID"];
            string btnNUM = Request.QueryString["EMID"];//取得要刪除的ID STRING
            int EmployeeID = Convert.ToInt32(btnNUM.Remove(0, 9));//轉為EMPLOYEE_ID
            PCGEntities db = new PCGEntities();
            Administrators ad = db.Administrators.Find(EmployeeID);
            return View(ad);
        }
        [HttpPost]
        public ActionResult Delete(Administrators ad)
        {
            PCGEntities db = new PCGEntities();
            db.Entry(ad).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("PMSTART");
        }
        //-----------------------管理人員資料刪除-----------------------------

        //
        [HttpPost]
        public ActionResult Search()
        {

            string searchname = Request["searchname"].ToString();
            int FacNo = Convert.ToInt32(Request["FacNo"].ToString());
            int ZoneID = Convert.ToInt32(Request["ZoneID"].ToString());
            //searchname = Request["searchname"].ToString();
            PCGEntities db = new PCGEntities();


            var query = from f in db.Factories
                        join p in db.Permission
                        on f.FacNo equals p.FacNo
                        join ad in db.Administrators
                        on p.EmployeeID equals ad.EmployeeID
                        where f.FacNo == FacNo && f.ZoneID == ZoneID //篩選廠別編號  地區編號
                        orderby p.EmployeeID
                        select new PersonInformation
                        {
                            FacName = f.FacName,
                            Character = ad.Character,
                            EmployeeID = p.EmployeeID,
                            Name = ad.Name,
                            ZoneID = f.ZoneID
                        };
            List<PersonInformation> model = new List<PersonInformation>();
            query = query.Where(x => x.Name.Contains(searchname));// 篩選姓名關鍵字
            foreach (var item in query) //retrieve each item and assign to model
            {
                model.Add(new PersonInformation()
                {
                    FacName = item.FacName,
                    EmployeeID = item.EmployeeID,
                    Character = item.Character,
                    Name = item.Name
                }
                );
            }
            return Json(new { model });
        }


        public ActionResult Permission()
        {
            //新增修改權限欄位
            PCGEntities db = new PCGEntities();
            List<PersonInformation> model = new List<PersonInformation>();
            var query = from z in db.Administrators
                        select new PersonInformation
                        {
                            Name = z.Name,
                            EmployeeID = z.EmployeeID
                        };
            foreach (var item in query) //retrieve each item and assign to model
            {
                model.Add(new PersonInformation()
                {
                    Name = item.Name,
                    EmployeeID = item.EmployeeID
                });
            }
            return View(model);

        }
        [HttpPost]
        public ActionResult Permission(int name)
        {
            name = Convert.ToInt32(Request["name"]);
            PCGEntities db = new PCGEntities();

            var query = from p in db.Permission
                        join f in db.Factories
                        on p.FacNo equals f.FacNo
                        where p.EmployeeID == name
                        select new PersonInformation
                        {
                            EmployeeID = p.EmployeeID,
                            FacName = f.FacName,
                            FacNo = f.FacNo

                        };
            List<PersonInformation> model = new List<PersonInformation>();

            foreach (var item in query)
            {
                model.Add(new PersonInformation
                {
                    EmployeeID = item.EmployeeID,
                    FacName = item.FacName,
                    FacNo = item.FacNo
                });
            };


            return Json(new { model });

        }
        [HttpPost]
        public ActionResult Permission2()
        {
            int EmployeeID = Convert.ToInt32(Request["EmployeeID"]);
            PCGEntities db = new PCGEntities();
            List<PersonInformation> model = new List<PersonInformation>();
            var query = (from f in db.Factories
                         join per in db.Permission
                         on f.FacNo equals per.FacNo
                         join ad in db.Administrators
                         on per.EmployeeID equals ad.EmployeeID
                         select new PersonInformation()
                         {
                             FacName = f.FacName
                         }).Except(
                from f in db.Factories
                join ad in db.Administrators
                on f.FacID equals ad.FacID
                where ad.EmployeeID == EmployeeID
                select new PersonInformation()
                {
                    FacName = f.FacName
                }
                );

            foreach (var item in query) {
                model.Add(new PersonInformation
                {
                    FacName = item.FacName

                });
            }

            //var havedquery=from f in db.Factories
            return Json(new { model });

        }
        [HttpPost]
        public ActionResult Permissionsubmit(FormCollection chk)
        {
            PCGEntities db = new PCGEntities();
            string[] values = chk.GetValues("HaveSelect");//取得要的權限的工廠名字

            for (var i = 0; i < values.Count(); i++) {
                int FacNo = Convert.ToInt32(values[i]);
                var queryzoneID = from f in db.Factories //查出挑出一項的zoneID
                                  where f.FacNo == FacNo
                                  select f.ZoneID;
                var queryFacNo = from f in db.Factories //查出挑出一項的zoneID
                                 where f.FacNo == FacNo
                                 select f.FacNo;
                var queryemp = (from em in db.Administrators
                                orderby em.EmployeeID descending
                                select em.EmployeeID).Take(1);//查出現在最高值

                var maxstringemp = queryemp;//取得新增之後的emp名
                int MaxIntEmp = maxstringemp.First();
                int IntFacNo = queryFacNo.First();
                int IntZone = (int)queryzoneID.First();
                Permission per = new Permission();
                per.EmployeeID = MaxIntEmp;
                per.ZoneID = IntZone;
                per.FacNo = IntFacNo;

                db.Entry(per).State = EntityState.Added;
                db.SaveChanges();
            }
            return RedirectToAction("PMSTART");
        }

        [HttpPost]
        public ActionResult EditPermissionsubmit(FormCollection chk,string FacID,string Name,string Character,string Email,string Account,string Password)
        {

            PCGEntities db = new PCGEntities();
            SqlConnection CN = new SqlConnection("server=pcgdb.c45myfrybea8.ap-northeast-1.rds.amazonaws.com;user=PCGdb;database=PCG;password=12345678;");
            int ZoneID = 0;
            string[] values = chk.GetValues("HaveSelect");//取得要的權限的工廠名字
            string[] FacIDvalues = chk.GetValues("FacID");
            string[] EmployeeIDvalues = chk.GetValues("EmployeeID");
            int EmployeeIDdel = Convert.ToInt32(EmployeeIDvalues[0]);
            string FacID2 = FacIDvalues[0];
            CN.Open();
            SqlCommand SCSelFacID = new SqlCommand("select zoneID from Factories where FacID = '"+FacID2+"'", CN);
            SqlDataReader SR = SCSelFacID.ExecuteReader();

            while (SR.Read()) {
               ZoneID=SR.GetInt32(0);

            }
            CN.Close();
            //刪除權限
            CN.Open();
            SqlCommand SCDEL = new SqlCommand("delete Permission where EmployeeID = "+ EmployeeIDdel,CN);
            SCDEL.ExecuteReader();
            CN.Close();
          

            ////加入權限
            for (int i = 0; i< values.Count(); i++) {
                CN.Open();
                SqlCommand SCADD = new SqlCommand("insert into Permission(EmployeeID, ZoneID, FacNo)VALUES("+EmployeeIDdel+ ", "+ZoneID+", " + values[i]+")", CN);
                SCADD.ExecuteReader();
                CN.Close();

            }
            CN.Open();
            SqlCommand SCEditAD = new SqlCommand("update Administrators set FacID = '"+FacID+"', name = N'"+ Name + "',[Character] = N'"+ Character + "', Email = '"+ Email + "', Account = '"+ Account + "',[Password] = '"+ Password + "'where EmployeeID = "+ EmployeeIDdel, CN);
            SCEditAD.ExecuteReader();
            CN.Close();
            

            //for (var i = 0; i < values.Count(); i++)
            //{
            //    int FacNo = Convert.ToInt32(values[i]);
            //    int EmployeeID = Convert.ToInt32(EmployeeIDvalues[i]);
            //    var queryzoneID = from f in db.Factories //查出挑出一項的ZoneID
            //                      where f.FacNo == FacNo
            //                      select f.ZoneID;
            //    var queryFacNo = from f in db.Factories //查出挑出一項的FacNo
            //                     where f.FacNo == FacNo
            //                     select f.FacNo;
            //    //查現在是哪一個EmployeeID
            //    int IntFacNo = queryFacNo.First();
            //    int IntZone = (int)queryzoneID.First();
            //    Permission per = new Permission();
            //    per.EmployeeID = EmployeeID;
            //    per.ZoneID = IntZone;
            //    per.FacNo = IntFacNo;

            //    db.Entry(per).State = EntityState.Added;
            //    db.SaveChanges();
            return RedirectToAction("PMSTART");
        }

    }



    }

