using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;
using System.Security.Claims;

namespace SV22T1020439.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 10;

        public IActionResult DonHang(int page = 1, int status = -1, string searchValue = "")
        {
            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(userStr)) return RedirectToAction("Login", "Account");
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            int customerId = int.Parse(user.UserId);

            int rowCount = OrderDataService.CountOrders(-1, null, null, searchValue); 
            var allOrders = OrderDataService.ListOrders(1, 10000, -1, null, null, searchValue); // Lấy tất cả đơn hàng
            
            // Filter theo status và khách hàng
            var customerOrders = allOrders.Where(x => x.CustomerID == customerId).ToList();
            if (status != -1)
            {
                customerOrders = customerOrders.Where(x => x.Status == status).ToList();
            }
            else
            {
                // Nếu là -1 (Tất cả), chỉ lấy các đơn hàng đang xử lý (0, 1, 2)
                customerOrders = customerOrders.Where(x => x.Status == 0 || x.Status == 1 || x.Status == 2).ToList();
            }
            
            rowCount = customerOrders.Count; // Cập nhật rowCount sau khi filter
            
            var data = customerOrders.OrderByDescending(x => x.OrderTime)
                                     .Skip((page - 1) * PAGE_SIZE)
                                     .Take(PAGE_SIZE).ToList();

            var model = new OrderSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue,
                Status = status,
                RowCount = customerOrders.Count,
                Data = data
            };
            return View("DonHang", model);
        }

        public IActionResult History(int page = 1, int status = -1, string searchValue = "")
        {
            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            if (string.IsNullOrEmpty(userStr)) return RedirectToAction("Login", "Account");
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            int customerId = int.Parse(user.UserId);

            int rowCount = OrderDataService.CountOrders(-1, null, null, searchValue); 
            var allOrders = OrderDataService.ListOrders(1, 10000, -1, null, null, searchValue); // Lấy tất cả đơn hàng
            
            // Filter theo status và khách hàng
            var customerOrders = allOrders.Where(x => x.CustomerID == customerId).ToList();
            if (status != -1)
            {
                customerOrders = customerOrders.Where(x => x.Status == status).ToList();
            }
            else
            {
                // Nếu là -1 (Tất cả), chỉ lấy các đơn hàng đã kết thúc (3, 4)
                customerOrders = customerOrders.Where(x => x.Status == 3 || x.Status == 4).ToList();
            }
            
            rowCount = customerOrders.Count; // Cập nhật rowCount sau khi filter
            
            var data = customerOrders.OrderByDescending(x => x.OrderTime)
                                     .Skip((page - 1) * PAGE_SIZE)
                                     .Take(PAGE_SIZE).ToList();

            var model = new OrderSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue,
                Status = status,
                RowCount = customerOrders.Count,
                Data = data
            };
            return View(model);
        }

        public IActionResult Details(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null) return RedirectToAction("History");

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            if (order.CustomerID != int.Parse(user.UserId)) return Forbid();

            order.Details = OrderDataService.ListDetails(id);
            return View(order);
        }

        public IActionResult Checkout()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            var customer = CommonDataService.GetCustomer(int.Parse(user.UserId));

            ViewBag.Customer = customer;
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(string deliveryProvince, string deliveryAddress)
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            int customerId = int.Parse(user.UserId);

            var details = cart.Select(x => new OrderDetail()
            {
                ProductID = x.ProductID,
                Quantity = x.Quantity,
                SalePrice = x.SalePrice
            });

            int orderId = OrderDataService.InitOrder(null, customerId, deliveryProvince, deliveryAddress, details);
            if (orderId > 0)
            {
                ShoppingCartService.ClearCart();
                TempData["Message"] = $"Đặt hàng thành công! Mã đơn hàng: #{orderId}";
                return RedirectToAction("DonHang"); // Chuyển đến trang Đơn Hàng
            }

            ModelState.AddModelError("", "Không thể lập đơn hàng. Vui lòng thử lại");
            return View("Checkout", cart);
        }

        public IActionResult Cancel(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null) return RedirectToAction("History");

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            if (order.CustomerID != int.Parse(user.UserId)) return Forbid();

            if (order.Status == 0 || order.Status == 1) // Chỉ cho phép hủy nếu là đơn mới hoặc đã duyệt
            {
                OrderDataService.CancelOrder(id);
                TempData["Message"] = "Đơn hàng #" + id + " đã được hủy thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng này vì đang ở trạng thái: " + order.StatusDescription;
            }

            return RedirectToAction("History");
        }

        public IActionResult DeleteDetail(int orderId, int productId)
        {
            var order = OrderDataService.GetOrder(orderId);
            if (order == null) return RedirectToAction("DonHang");

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            if (order.CustomerID != int.Parse(user.UserId)) return Forbid();

            if (order.Status != 0)
            {
                TempData["ErrorMessage"] = "Chỉ có thể xóa chi tiết đơn hàng mới (chờ duyệt).";
                return RedirectToAction("Details", new { id = orderId });
            }

            // Xóa chi tiết đơn hàng
            OrderDataService.DeleteOrderDetail(orderId, productId);
            TempData["Message"] = "Xóa chi tiết đơn hàng thành công.";
            return RedirectToAction("Details", new { id = orderId });
        }

        [HttpPost]
        public IActionResult UpdateAllDetails(int orderId, List<OrderDetail> details)
        {
            var order = OrderDataService.GetOrder(orderId);
            if (order == null) return RedirectToAction("DonHang");

            var userStr = User.FindFirst(ClaimTypes.UserData)?.Value;
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(userStr);
            if (order.CustomerID != int.Parse(user.UserId)) return Forbid();

            if (order.Status != 0)
            {
                TempData["ErrorMessage"] = "Chỉ có thể sửa đơn hàng mới (chờ duyệt).";
                return RedirectToAction("Details", new { id = orderId });
            }

            // Cập nhật từng chi tiết
            foreach (var detail in details)
            {
                if (detail.Quantity > 0)
                {
                    OrderDataService.UpdateOrderDetail(orderId, detail.ProductID, detail.Quantity, detail.SalePrice);
                }
            }

            TempData["Message"] = "Cập nhật số lượng thành công.";
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
