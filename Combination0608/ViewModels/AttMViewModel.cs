using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;

namespace Combination0608.ViewModels
{
    public class AttMViewModel
    {
        //放置檢索關鍵字
        public AttMSearchViewModel SearchParaMeter { set; get; }

        public IPagedList<FacPopulation> Populations { set; get; }

        public IPagedList<AttM> AttMSearchList { set; get; }

        //下拉式選單的值
        public SelectList ZoneID { set; get; }

        public SelectList Factory { set; get; }

        public SelectList Date { set; get; }

        //分頁功能
        public int PageIndex { set; get; }

        public int search { set; get; }
    }
}