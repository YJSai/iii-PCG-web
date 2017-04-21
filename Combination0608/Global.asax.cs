using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Combination0608.Models;
using Hangfire;

namespace Combination0608
{
    public class MvcApplication : System.Web.HttpApplication
    {
        PCGEntities db = new PCGEntities();
        CheckEmp sm = new CheckEmp();

        private BackgroundJobServer _backgroundJobServer;

        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration
.UseSqlServerStorage(@"server=.\sqlexpress;user=sa;database=PCG;password=123;");
            //            GlobalConfiguration.Configuration
            //.UseSqlServerStorage("server=pcgddb.c45myfrybea8.ap-northeast-1.rds.amazonaws.com;user=PCGdb;database=PCG;password=12345678;");

            _backgroundJobServer = new BackgroundJobServer();

            string email = "taipeibigegg@gmail.com";
            //時間到呼叫檢查方法
            //RecurringJob.AddOrUpdate(() => sm.Check(), "36 08 * * *");    //每天幾點幾分
            //RecurringJob.AddOrUpdate(() => sm.Check(), "0 * * * *");    //每天每小時
            //RecurringJob.AddOrUpdate(() => sm.Check(email), "0 02 * * *");    //每天早上9點
            //RecurringJob.AddOrUpdate(() => sm.Check(), "0 15 * * *");    //每天下午3點


            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_End(object sender, EventArgs e) {
            _backgroundJobServer.Dispose();
        }

        protected void Session_Start()
        {
            //Session.Add("EmployeeName", "Guest");
            Session.Add("ReturnUrl", "/Login/Login");
            //Session.Add("EmployeeID", 0);
        }
    }
}
