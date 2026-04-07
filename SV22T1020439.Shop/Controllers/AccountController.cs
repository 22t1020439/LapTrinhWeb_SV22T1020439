using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;
using System.Security.Claims;

namespace SV22T1020439.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập Email và Mật khẩu");
                return View();
            }
            
            // Xác minh email và mat khau
            var customer = UserAccountService.Authorize(UserAccountService.UserTypes.Customer, email, password);
            if (customer == null)
            {
                ModelState.AddModelError("", "Đăng nhập không thành công, liên hệ hoặc đợi Admin duyệt tài khoản");
                 return View();
            }

            // Kiểm tra tài khoản có bị khóa không sau khi xác minh thành công
            var customerInfo = CommonDataService.ListOfCustomers(1, 0, email).FirstOrDefault();
            if (customerInfo != null && customerInfo.IsLocked)
            {
                ModelState.AddModelError("", "Dang nhap that bai, liên he hoac doi Admin duyet tài khoan");
                return View();
            }

            var userData = new UserAccount()
            {
                UserId = customer.UserId,
                UserName = customer.UserName,
                DisplayName = customer.DisplayName,
                Photo = customer.Photo,
                RoleNames = customer.RoleNames
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userData.UserId),
                new Claim(ClaimTypes.GivenName, userData.DisplayName),
                new Claim(ClaimTypes.Role, userData.RoleNames),
                new Claim(ClaimTypes.UserData, Newtonsoft.Json.JsonConvert.SerializeObject(userData))
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new Customer() { CustomerID = 0 });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Customer data, string confirmPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Email không được để trống");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(data.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                ModelState.AddModelError(nameof(data.Email), "Email/Gmail không hợp lệ");
            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError(nameof(password), "Mật khẩu không được để trống");
            if (password != confirmPassword)
                ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu xác nhận không khớp");

            // Kiểm tra số điện thoại
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"^0\d{9}$"))
            {
                ModelState.AddModelError(nameof(data.Phone), "SDT không hợp lệ");
            }

            if (!ModelState.IsValid)
                return View(data);

            // Check email exist
            if (CommonDataService.ListOfCustomers(1, 0, data.Email).Any(x => x.Email == data.Email))
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại với Email/Gmail khác");
                return View(data);
            }

            data.Password = password; 
            data.IsLocked = true;
            
            int customerId = CommonDataService.AddCustomer(data);
            if (customerId > 0)
            {
                TempData["Message"] = "Đăng ký tài khoản thành công! Tài khoản của bạn đang bị khóa chờ admin duyệt.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại với Email/Gmail khác");
            return View(data);
        }

        public IActionResult Profile()
        {
            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(userStr)) return RedirectToAction("Login");
            
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            int customerId = int.Parse(user.UserId);
            
            var customer = CommonDataService.GetCustomer(customerId);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            
            // Kiểm tra số điện thoại
            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"^0\d{9}$"))
            {
                ModelState.AddModelError(nameof(data.Phone), "SDT không hợp lệ");
            }
            
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Cập nhật thông tin thất bại. Nhập đúng Số điện thoại.";
                return View("Profile", data);
            }

            CommonDataService.UpdateCustomer(data);
            
            TempData["Message"] = "Cập nhật thông tin cá nhân thành công!";
            return RedirectToAction("Profile");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ mật khẩu");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Mật khẩu xác nhận không khớp");
                return View();
            }

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(userStr)) return RedirectToAction("Login");
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);

            var checkUser = UserAccountService.Authorize(UserAccountService.UserTypes.Customer, user.UserName, oldPassword);
            if (checkUser == null)
            {
                ModelState.AddModelError("oldPassword", "Mật khẩu cũ không chính xác");
                return View();
            }

            bool result = UserAccountService.ChangePassword(UserAccountService.UserTypes.Customer, user.UserName, newPassword);
            if (result)
            {
                TempData["Message"] = "Đổi mật khẩu thành công";
                return RedirectToAction("ChangePassword");
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi đổi mật khẩu";
            return RedirectToAction("ChangePassword");
        }
    }
}
