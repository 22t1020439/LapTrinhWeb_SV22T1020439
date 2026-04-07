using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SV22T1020439.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.ORDER}")]
    public class OrderController : Controller
    {
        public const int PAGE_SIZE = 20;
        private const string DATE_FORMAT = "dd/MM/yyyy";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EditCartItem(int productId = 0)
        {
            var item = ShoppingCartService.GetCartItem(productId);
            if (item == null)
                return NotFound();
            return PartialView(item);
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, int quantity, decimal salePrice)
        {
            if (quantity <= 0 || salePrice < 0)
                return Json(new { success = false, message = "Số lượng hoặc giá không hợp lệ" });

            ShoppingCartService.UpdateCartItem(productId, quantity, salePrice);
            return Json(new { success = true });
        }

        public IActionResult RemoveFromCart(int productId = 0)
        {
            ShoppingCartService.RemoveCartItem(productId);
            return Json(new { success = true });
        }

        public IActionResult Search(int page = 1, int status = -1, string fromTime = "", string toTime = "", string searchValue = "")
        {
            DateTime? fromDate = null;
            DateTime? toDate = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(fromTime))
                    fromDate = DateTime.ParseExact(fromTime, DATE_FORMAT, CultureInfo.InvariantCulture);
                if (!string.IsNullOrWhiteSpace(toTime))
                    toDate = DateTime.ParseExact(toTime, DATE_FORMAT, CultureInfo.InvariantCulture);
            }
            catch { }

            int rowCount = OrderDataService.CountOrders(status, fromDate, toDate, searchValue ?? "");
            var data = OrderDataService.ListOrders(page, PAGE_SIZE, status, fromDate, toDate, searchValue ?? "");

            var model = new OrderSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                Status = status,
                FromTime = fromTime,
                ToTime = toTime,
                RowCount = rowCount,
                Data = data
            };

            return PartialView(model);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            order.Details = OrderDataService.ListDetails(id);
            return View("Detail", order);
        }

        public IActionResult Accept(int id = 0)
        {
            OrderDataService.AcceptOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Shipping(int id = 0, int shipperID = 0, string shippedTime = "")
        {
            if (Request.Method == "POST")
            {
                if (shipperID <= 0)
                {
                    ModelState.AddModelError("ShipperID", "Vui lòng chọn người giao hàng");
                }
                
                DateTime? shippedDate = null;
                if (!string.IsNullOrWhiteSpace(shippedTime))
                {
                    try
                    {
                        shippedDate = DateTime.ParseExact(shippedTime, DATE_FORMAT, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        ModelState.AddModelError("ShippedTime", "Ngày giao hàng không hợp lệ");
                    }
                }
                else
                {
                    ModelState.AddModelError("ShippedTime", "Vui lòng chọn ngày giao hàng");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.OrderID = id;
                    ViewBag.Shippers = CommonDataService.ListOfShippers();
                    return PartialView();
                }

                OrderDataService.ShipOrder(id, shipperID, shippedDate.Value);
                return Json(new { success = true });
            }
            ViewBag.OrderID = id;
            ViewBag.Shippers = CommonDataService.ListOfShippers();
            return PartialView();
        }

        public IActionResult Finish(int id = 0)
        {
            OrderDataService.FinishOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Cancel(int id = 0)
        {
            OrderDataService.CancelOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Reject(int id = 0)
        {
            OrderDataService.RejectOrder(id);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                OrderDataService.DeleteOrder(id);
                return RedirectToAction("Index");
            }
            var model = OrderDataService.GetOrder(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            if (model == null)
                return NotFound();
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ" });

            OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            return Json(new { success = true });
        }

        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            OrderDataService.DeleteOrderDetail(id, productId);
            return RedirectToAction("Details", new { id = id });
        }

        public IActionResult Create()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return View(cart);
        }

        public IActionResult ShoppingCart()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return PartialView("_ShoppingCart", cart);
        }

        public IActionResult SearchProduct(int page = 1, string searchValue = "", int minPrice = 0, int maxPrice = 0)
        {
            int rowCount = ProductDataService.CountProducts(searchValue, 0, 0, minPrice, maxPrice);
            var data = ProductDataService.ListOfProducts(page, 5, searchValue, 0, 0, minPrice, maxPrice); // PageSize = 5 for search sidebar
            var model = new PaginationSearchResult<Product>()
            {
                Page = page,
                PageSize = 5,
                SearchValue = searchValue,
                RowCount = rowCount,
                Data = data
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddToCart(CartItem item)
        {
            if (item.Quantity <= 0)
                return Json(new { success = false, message = "Số lượng không hợp lệ" });

            ShoppingCartService.AddCartItem(item);
            return Json(new { success = true });
        }

        public IActionResult ClearCart()
        {
            ShoppingCartService.ClearCart();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Init(int? customerID, string deliveryProvince, string deliveryAddress)
        {
            var cart = ShoppingCartService.GetShoppingCart();
            if (cart.Count == 0)
                return Json(new { success = false, message = "Giỏ hàng trống" });

            // Create order details from cart
            var details = cart.Select(x => new OrderDetail()
            {
                ProductID = x.ProductID,
                Quantity = x.Quantity,
                SalePrice = x.SalePrice
            });

            // Lấy ID nhân viên từ Identity
            int employeeID = 0;
            if (User.Identity != null && int.TryParse(User.Identity.Name, out int id))
            {
                employeeID = id;
            }

            int? finalCustomerID = customerID > 0 ? customerID : null;

            int orderID = OrderDataService.InitOrder(employeeID, finalCustomerID, deliveryProvince ?? "", deliveryAddress ?? "", details);
            if (orderID > 0)
            {
                ShoppingCartService.ClearCart();
                return Json(new { success = true, orderID = orderID });
            }

            return Json(new { success = false, message = "Không lập được đơn hàng" });
        }
    }
}
