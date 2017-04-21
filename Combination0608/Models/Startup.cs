using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(Combination0608.Models.Startup))]

namespace Combination0608.Models {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("server=pcgddb.c45myfrybea8.ap-northeast-1.rds.amazonaws.com;user=PCGdb;database=PCG;password=12345678;");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}