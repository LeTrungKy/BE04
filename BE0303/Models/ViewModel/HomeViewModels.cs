using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE0303.Models.ViewModel
{
    // Số sản phẩm của một danh mục (dùng cho biểu đồ tròn)
    public class CategoryStat
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    // Dữ liệu thống kê cho trang Home của Admin
    public class HomeStatsVM
    {
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int CustomerCount { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public List<CategoryStat> CategoryStats { get; set; }
    }
}
