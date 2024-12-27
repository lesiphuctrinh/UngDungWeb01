using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.Shop.AppCodes;

namespace SV21T1020777.Shop.Controllers
{
    [Authorize]
    public class ShopAccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet("/ShopAccount/Login")]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost("/ShopAccount/Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;
            //Kiểm tra dữ liệu dầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu!");
                return View();
            }
            //Kiểm tra xem username và password có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
            if (userAccount == null)
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
            return RedirectToAction("Index", "ShopHome");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult MyAccount()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UpdateDetails()
        {
            TempData["ActiveTab"] = "ShopAccount-details";
            return View("MyAccount");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDetails(string displayName, string email, string phone, IFormFile _Photo, string PhotoCurrent)
        {
            try
            {
                var userData = User.GetUserData();
                if (userData == null)
                {
                    TempData["Error1"] = "Vui lòng đăng nhập.";
                    TempData["ActiveTab"] = "ShopAccount-details";
                    return RedirectToAction("MyAccount");
                }

                if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
                {
                    TempData["Error1"] = "Vui lòng nhập đầy đủ thông tin.";
                    TempData["ActiveTab"] = "ShopAccount-details";
                    return RedirectToAction("MyAccount");
                }

                if (!email.Contains("@") || !email.Contains("."))
                {
                    TempData["Error1"] = "Email không hợp lệ.";
                    TempData["ActiveTab"] = "ShopAccount-details";
                    return RedirectToAction("MyAccount");
                }

                // xử lí ảnh
                string Photo = PhotoCurrent; // lấy ảnh cũ nếu như không chọn ảnh
                if (_Photo != null)
                {
                    // Lưu ảnh
                    string fileName = $"{DateTime.Now.Ticks}-{_Photo.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images/customer", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _Photo.CopyTo(stream);
                    }
                    Photo = fileName;
                }
                bool result = UserAccountService.UpdateAccountDetails(UserTypes.Customer, userData.UserId, displayName ,email, phone, Photo);
                if (result)
                {
                    TempData["Success1"] = "Cập nhật thông tin thành công.";
                }
                else
                {
                    TempData["Error1"] = "Cập nhật thông tin thất bại. Vui lòng thử lại.";
                }
                TempData["ActiveTab"] = "ShopAccount-details";
                return RedirectToAction("MyAccount");
            }
            catch (Exception ex)
            {
                TempData["ActiveTab"] = "ShopAccount-details";
                return View("MyAccount");
            }
  
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            TempData["ActiveTab"] = "ShopAccount-change";
            return View("MyAccount");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userData = User.GetUserData();
            if (userData == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập.";
                TempData["ActiveTab"] = "ShopAccount-change";
                return RedirectToAction("MyAccount");
            }

            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                TempData["ActiveTab"] = "ShopAccount-change";
                return RedirectToAction("MyAccount");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp.";
                TempData["ActiveTab"] = "ShopAccount-change";
                return RedirectToAction("MyAccount");
            }

            var userAccount = UserAccountService.Authorize(UserTypes.Customer, userData.UserName, oldPassword);
            if (userAccount == null)
            {
                TempData["Error"] = "Mật khẩu cũ không chính xác.";
                TempData["ActiveTab"] = "ShopAccount-change";
                return RedirectToAction("MyAccount");
            }

            bool result = UserAccountService.ChangePassword(UserTypes.Customer, userData.UserName, newPassword);
            if (result)
            {
                TempData["Success"] = "Đổi mật khẩu thành công.";
            }
            else
            {
                TempData["Error"] = "Đổi mật khẩu thất bại. Vui lòng thử lại.";
            }
            TempData["ActiveTab"] = "ShopAccount-change";
            return RedirectToAction("MyAccount");
        }

    }
}
