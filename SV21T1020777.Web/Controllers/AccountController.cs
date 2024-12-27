using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;

namespace SV21T1020777.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            //Kiểm tra dữ liệu đầu vào
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }
            //TODO: Kiểm tra xem username và password có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if(userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            //Đăng nhập thành công: ghi nhận trạng thái đăng nhập
            // 1. tạo thông tin của người dùng
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Phone = userAccount.Phone,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(",").ToList()
            };
            //2. ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(userData.CreatePrincipal());

            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenined()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            //Lấy thông tin đăng nhập
            var userData = User.GetUserData();
            if(userData == null)
            {
                return RedirectToAction("Login");
            }
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("Error", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Error", "Mật khẩu xác nhận không khớp.");
                return View();
            }

            // Xác thực mật khẩu cũ
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, userData.UserName, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Mật khẩu cũ không chính xác.");
                return View();
            }

            // Đổi mật khẩu
            bool result = UserAccountService.ChangePassword(UserTypes.Employee, userData.UserName, newPassword);
            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công.";
                return View();
            }

            ModelState.AddModelError("Error", "Đổi mật khẩu thất bại. Vui lòng thử lại.");
            return View();

        }

    }
}
