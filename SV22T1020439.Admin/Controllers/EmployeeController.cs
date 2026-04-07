using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;
using System;

namespace SV22T1020439.Admin.Controllers
{
    [Authorize(Roles = WebUserRoles.ADMIN)]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 20;

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = CommonDataService.CountEmployees(searchValue ?? "");
            var data = CommonDataService.ListOfEmployees(page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchResult<Employee>()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee()
            {
                EmployeeID = 0,
                IsWorking = true,
                Photo = "avatar.jpg"
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = HRDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveData(Employee data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

                //Kiểm tra dữ liệu đầu vào: FullName và Email là bắt buộc, Email chưa được sử dụng bởi nhân viên khác
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Vui lòng nhập họ tên nhân viên");

                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email nhân viên");
                else if (!HRDataService.ValidateEmployeeEmail(data.Email, data.EmployeeID))
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng bởi nhân viên khác");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                //Xử lý upload ảnh
                if (uploadPhoto != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                    var filePath = Path.Combine(ApplicationContext.WWWRootPath, "images/employees", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                //Tiền xử lý dữ liệu trước khi lưu vào database
                if (string.IsNullOrEmpty(data.Address)) data.Address = "";
                if (string.IsNullOrEmpty(data.Phone)) data.Phone = "";
                if (string.IsNullOrEmpty(data.Photo)) data.Photo = "avatar.jpg";

                //Lưu dữ liệu vào database (bổ sung hoặc cập nhật)
                if (data.EmployeeID == 0)
                {
                    // Gán quyền mặc định cho nhân viên mới: quản lý khách hàng và lập đơn hàng
                    data.RoleNames = "customer,order";
                    HRDataService.AddEmployee(data);
                }
                else
                {
                    HRDataService.UpdateEmployee(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //TODO: Ghi log lỗi căn cứ vào ex.Message và ex.StackTrace
                ModelState.AddModelError(string.Empty, "Hệ thống đang bận hoặc dữ liệu không hợp lệ. Vui lòng kiểm tra dữ liệu hoặc thử lại sau");
                return View("Edit", data);
            }
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool isUsed = CommonDataService.IsUsedEmployee(id);
                if (isUsed)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhân viên này vì đã có dữ liệu liên quan (đơn hàng).";
                    return RedirectToAction("Delete", new { id = id });
                }

                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.IsUsed = CommonDataService.IsUsedEmployee(id);
            return View(model);
        }

        public IActionResult ChangePassword(int id = 0)
        {
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangePassword(int id, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Password", "Mật khẩu không được để trống");
                var model = CommonDataService.GetEmployee(id);
                return View(model);
            }
            CommonDataService.ChangeEmployeePassword(id, password);
            return RedirectToAction("Index");
        }

        public IActionResult ChangeRole(int id = 0)
        {
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult ChangeRole(int id, string[] roleNames)
        {
            string roles = "";
            if (roleNames != null && roleNames.Length > 0)
            {
                roles = string.Join(",", roleNames);
            }
            CommonDataService.ChangeEmployeeRole(id, roles);
            return RedirectToAction("Index");
        }
    }
}
