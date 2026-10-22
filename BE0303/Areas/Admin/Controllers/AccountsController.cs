using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE0303.Filters;
using BE0303.Models;
using BE0303.Models.ViewModel;
using PagedList;

using AdminUser = BE0303.Models.User;

namespace BE0303.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class AccountsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Accounts

        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var model = new AccountSearchVM();
            var accounts = db.Users.Where(u => u.UserRole == AdminAuth.Role);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model.SearchTerm = searchTerm;
                accounts = accounts.Where(u => u.Username.Contains(searchTerm));
            }

            model.SortOrder = sortOrder;
            if (sortOrder == "user_desc")
            {
                accounts = accounts.OrderByDescending(u => u.Username);
            }
            else
            {
                accounts = accounts.OrderBy(u => u.Username);
            }

            model.PageNumber = page ?? 1;
            model.PageSize = 5;
            model.Accounts = accounts.ToPagedList(model.PageNumber, model.PageSize);

            return View(model);
        }

        // GET: Admin/Accounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password")] AdminUser account)
        {
            if (string.IsNullOrWhiteSpace(account.Username))
            {
                ModelState.AddModelError("Username", "Vui lòng nhập tên đăng nhập");
            }
            if (string.IsNullOrWhiteSpace(account.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu");
            }
            if (!string.IsNullOrWhiteSpace(account.Username))
            {
                account.Username = account.Username.Trim();
                if (db.Users.Any(u => u.Username == account.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                }
            }

            if (ModelState.IsValid)
            {
                account.UserRole = AdminAuth.Role;
                db.Users.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Admin/Accounts/Edit/admin1
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser account = db.Users.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Edit/admin1
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password")] AdminUser account)
        {
            AdminUser existing = db.Users.Find(account.Username);
            if (existing == null)
            {
                return HttpNotFound();
            }

            if (!string.IsNullOrWhiteSpace(account.Password))
            {
                existing.Password = account.Password;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Accounts/Delete/admin1
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminUser account = db.Users.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Admin/Accounts/Delete/admin1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AdminUser account = db.Users.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            
            string current = Session[AdminAuth.SessionKey] as string;
            if (string.Equals(id.Trim(), current, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Không thể xóa tài khoản đang đăng nhập.");
                return View("Delete", account);
            }

            try
            {
                db.Users.Remove(account);
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                
                ModelState.AddModelError("", "Không thể xóa tài khoản này vì đang được sử dụng ở dữ liệu khác.");
                return View("Delete", account);
            }
            return RedirectToAction("Index");
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
