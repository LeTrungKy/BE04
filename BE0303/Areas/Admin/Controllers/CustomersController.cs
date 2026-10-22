using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE0303.Models;
using BE0303.Models.ViewModel;
using PagedList;
using BE0303.Filters;

namespace BE0303.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Customers
        // Xem, tìm kiếm, sắp xếp, phân trang danh sách khách hàng
        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var model = new CustomerSearchVM();
            var customers = db.Customers.AsQueryable();

            // Tìm kiếm theo tên, số điện thoại, email, tên đăng nhập
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model.SearchTerm = searchTerm;
                customers = customers.Where(c => c.CustomerName.Contains(searchTerm)
                                              || c.CustomerPhone.Contains(searchTerm)
                                              || c.CustomerEmail.Contains(searchTerm)
                                              || c.Username.Contains(searchTerm));
            }

            // Sắp xếp
            model.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "name_asc":
                    customers = customers.OrderBy(c => c.CustomerName);
                    break;
                case "name_desc":
                    customers = customers.OrderByDescending(c => c.CustomerName);
                    break;
                case "email_asc":
                    customers = customers.OrderBy(c => c.CustomerEmail);
                    break;
                case "email_desc":
                    customers = customers.OrderByDescending(c => c.CustomerEmail);
                    break;
                default:
                    customers = customers.OrderBy(c => c.CustomerID);
                    break;
            }

            // Phân trang
            model.PageNumber = page ?? 1;
            model.PageSize = 5;
            model.Customers = customers.ToPagedList(model.PageNumber, model.PageSize);

            return View(model);
        }

        // GET: Admin/Customers/Details/5
        // Thông tin khách hàng và danh sách đơn hàng của khách hàng
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            var model = new CustomerOrdersVM
            {
                Customer = customer,
                Orders = db.Orders
                           .Where(o => o.CustomerID == id.Value)
                           .OrderByDescending(o => o.OrderDate)
                           .ToList()
            };
            return View(model);
        }

        // GET: Admin/Customers/OrderDetails/5   (5 là OrderID)
        // Chi tiết một đơn hàng
        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new OrderDetailsVM
            {
                Order = order,
                Customer = db.Customers.Find(order.CustomerID),
                Items = db.OrderDetails
                          .Include(d => d.Product)
                          .Where(d => d.OrderID == id.Value)
                          .ToList()
            };
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
