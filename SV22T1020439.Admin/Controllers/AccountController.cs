using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.Admin.Models;
using SV22T1020439.Admin.Models.Security;
using SV22T1020439.BusinessLayers;
using System.Security.Claims;

namespace SV22T1020439.Admin.Controllers
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
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Mã hóa mật khẩu trước khi kiểm tra
            string hashedPassword = CryptHelper.HashMD5(model.Password);

            // Kiểm tra thông tin đăng nhập từ Service
            var userAccount = UserAccountService.Authorize(UserAccountService.UserTypes.Employee, model.Username, hashedPassword);

            if (userAccount != null)
            {
                // 1. Tạo danh sách các Claims cho User
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userAccount.UserId),
                    new Claim(ClaimTypes.GivenName, userAccount.DisplayName),
                    new Claim(ClaimTypes.UserData, Newtonsoft.Json.JsonConvert.SerializeObject(userAccount))
                };

                // Thêm các Roles của user vào Claims
                if (!string.IsNullOrEmpty(userAccount.RoleNames))
                {
                    var roles = userAccount.RoleNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
                    }
                }

                // 2. Tạo Identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 3. Tạo Principal
                var principal = new ClaimsPrincipal(identity);

                // 4. Thiết lập các tùy chọn cho Authentication
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                // 5. Thực hiện đăng nhập (Lưu Cookie)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", $"Đăng nhập thất bại. Username: {model.Username}");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            // Lấy thông tin user đang đăng nhập từ Claims
            var userDataClaim = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(userDataClaim))
            {
                return RedirectToAction("Login");
            }

            var userAccount = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userDataClaim);
            if (userAccount == null)
            {
                return RedirectToAction("Login");
            }

            int employeeId = int.Parse(userAccount.UserId);

            // Kiểm tra mật khẩu cũ
            string hashedOldPassword = CryptHelper.HashMD5(oldPassword);
            var employee = CommonDataService.GetEmployee(employeeId);
            if (employee == null || employee.Password != hashedOldPassword)
            {
                ModelState.AddModelError("", "Mật khẩu cũ không đúng");
                return View();
            }

            // Kiểm tra mật khẩu mới
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError("", "Mật khẩu mới không được để trống");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                return View();
            }

            // Đổi mật khẩu
            string hashedNewPassword = CryptHelper.HashMD5(newPassword);
            bool result = CommonDataService.ChangeEmployeePassword(employeeId, hashedNewPassword);
            
            if (result)
            {
                ViewBag.SuccessMessage = "Đổi mật khẩu thành công";
                return View();
            }
            else
            {
                ModelState.AddModelError("", "Đổi mật khẩu thất bại. Vui lòng thử lại");
                return View();
            }
        }
    }
}
