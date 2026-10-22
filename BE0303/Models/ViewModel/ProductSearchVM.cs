using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace BE0303.Models.ViewModel
{
    public class ProductSearchVM
    {
       
        public string SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

      
        public string SortOrder { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public IPagedList<Product> Products { get; set; }
    }
}