using System;
using System.Linq;
using System.Web.Mvc;
using BE0303.Filters;
using BE0303.Models;
using BE0303.Models.ViewModel;

namespace BE0303.Areas.Admin.Controllers
{
    public class AuthController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Auth/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        // POST: Admin/Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var account = db.Users.FirstOrDefault(u => u.Username == model.Username
                                                        && u.Password == model.Password);

                bool isAdmin = account != null
                            && account.UserRole != null
                            && string.Equals(account.UserRole.Trim(), AdminAuth.Role, StringComparison.OrdinalIgnoreCase);

                if (isAdmin)
                {
                    Session[AdminAuth.SessionKey] = account.Username.Trim();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Categories");
                }

                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu, hoặc tài khoản không có quyền Admin.");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        // GET: Admin/Auth/Logout
        public ActionResult Logout()
        {
            Session.Remove(AdminAuth.SessionKey);
            Session.Abandon();
            return RedirectToAction("Login");
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
