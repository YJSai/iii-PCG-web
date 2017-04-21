using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Combination0608.ViewModels
{
    public class AttMSearchViewModel
    {
        //搜尋會用到的關鍵字：地區、廠別、日期
        public int ZoneID { get; set; }

        public string FacID { set; get; }

        public string Date { get; set; }

        public string Date2 { get; set; }
    }
}