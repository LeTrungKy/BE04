using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace BE0303.Filters
{
    // Các hằng số dùng chung cho phần đăng nhập Admin
    public static class AdminAuth
    {
        // Giá trị cột UserRole của tài khoản Admin trong bảng User.
        // Nếu dữ liệu của bạn dùng giá trị khác (ví dụ "admin", "1"), sửa ở đây.
        public const string Role = "Admin";

        // Tên khóa lưu tên đăng nhập trong Session
        public const string SessionKey = "AdminUser";
    }

    // Gắn [AdminAuthorize] lên controller của khu vực Admin để bắt buộc đăng nhập
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session[AdminAuth.SessionKey] == null)
            {
                string returnUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        area = "Admin",
                        controller = "Auth",
                        action = "Login",
                        returnUrl = returnUrl
                    }));
            }
        }
    }
}
