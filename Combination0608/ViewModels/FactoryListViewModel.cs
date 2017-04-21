using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Combination0608.Models;

namespace Combination0608.ViewModels {
    public class FactoryListViewModel {

        public FactorySearchViewModel SearchParaMeter { set; get; }

        public IPagedList<Factories> 廠別s { set; get; }

        public IPagedList<AttS> AttSearchList { set; get; }

        public SelectList ZoneID { set; get; }

        public SelectList Factory { set; get; }

        public int PageIndex { set; get; }

        public int search { set; get; }
    }

}