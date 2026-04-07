using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;

namespace SV22T1020439.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.CUSTOMER}")]
    public class CustomerController : Controller
    {
        private const int PAGE_SIZE = 20;

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            int rowCount = CommonDataService.CountCustomers(searchValue ?? "");
            var data = CommonDataService.ListOfCustomers(page, PAGE_SIZE, searchValue ?? "");
            var model = new PaginationSearchResult<Customer>()
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
            ViewBag.Title = "Bổ sung khách hàng";
            var model = new Customer()
            {
                CustomerID = 0
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
            
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.ContactName))
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Tỉnh/thành không được để trống");
            
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (data.CustomerID == 0)
            {
                CommonDataService.AddCustomer(data);
            }
            else
            {
                CommonDataService.UpdateCustomer(data);
            }
            return RedirectToAction("Index");
        }
        //dùng aysnc
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool isUsed = CommonDataService.IsUsedCustomer(id);
                if (isUsed)
                {
                    TempData["ErrorMessage"] = "Không thể xóa khách hàng này vì đã có dữ liệu liên quan (đơn hàng).";
                    return RedirectToAction("Delete", new { id = id });
                }

                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            var model = CommonDataService.GetCustomer(id);
            if (model == null)
                return RedirectToAction("Index");

            ViewBag.IsUsed = CommonDataService.IsUsedCustomer(id);
            return View(model);
        }

        public IActionResult ChangePassword(int id = 0)
        {
            var model = CommonDataService.GetCustomer(id);
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
                var model = CommonDataService.GetCustomer(id);
                return View(model);
            }
            CommonDataService.ChangeCustomerPassword(id, password);
            return RedirectToAction("Index");
        }
    }
}
