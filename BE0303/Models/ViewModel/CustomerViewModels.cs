using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace BE0303.Models.ViewModel
{
    // Tìm kiếm, sắp xếp, phân trang danh sách khách hàng
    public class CustomerSearchVM
    {
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }   // name_asc, name_desc, email_asc, email_desc
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IPagedList<Customer> Customers { get; set; }
    }

    // Thông tin khách hàng và danh sách đơn hàng của khách hàng đó
    public class CustomerOrdersVM
    {
        public Customer Customer { get; set; }
        public List<Order> Orders { get; set; }
    }

    // Chi tiết một đơn hàng: thông tin đơn, khách hàng và các sản phẩm trong đơn
    public class OrderDetailsVM
    {
        public Order Order { get; set; }
        public Customer Customer { get; set; }
        public List<OrderDetail> Items { get; set; }
    }
}
