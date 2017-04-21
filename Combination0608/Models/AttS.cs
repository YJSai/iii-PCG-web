using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Combination0608.Models {
    public class AttS {
        //AttS搜尋功能成員
        public Nullable<int> ZoneID { get; set; }
        public string Country { set; get; }
        public int FacNo { set; get; }
        public string FacName { set; get; }
        public string Character { set; get; }
        public string EmpName { set; get; }
        public string EmpEmail { set; get; }
        public Nullable<System.DateTime> EditTime { get; set; }
        public string FacID { get; set; }
        public Nullable<int> Supervisor { get; set; }
        public int EmployeeID { get; set; }
    }
}