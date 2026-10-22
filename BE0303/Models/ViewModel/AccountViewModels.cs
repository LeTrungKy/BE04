using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;

namespace BE0303.Models.ViewModel
{
    // Dữ liệu form đăng nhập
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
    }

    // Tìm kiếm, sắp xếp, phân trang danh sách tài khoản Admin
    public class AccountSearchVM
    {
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }   // user_asc, user_desc
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IPagedList<User> Accounts { get; set; }
    }
}
