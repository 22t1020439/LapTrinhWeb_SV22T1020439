using Microsoft.AspNetCore.Mvc;
using SV22T1020439.Models;
using SV22T1020439.BusinessLayers;
using System.Linq;

namespace SV22T1020439.Shop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = ProductDataService.GetProduct(productId);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            var item = new CartItem()
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Unit = product.Unit,
                Photo = product.Photo,
                SalePrice = product.Price,
                Quantity = quantity
            };

            ShoppingCartService.AddCartItem(item);
            return RedirectToAction("Index"); // Chuyển về trang giỏ hàng
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                ShoppingCartService.RemoveCartItem(productId);
            }
            else
            {
                ShoppingCartService.UpdateCartItem(productId, quantity);
            }
            return Json(new { success = true });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            ShoppingCartService.RemoveCartItem(productId);
            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            ShoppingCartService.ClearCart();
            return RedirectToAction("Index");
        }

        public IActionResult GetCartCount()
        {
            var cart = ShoppingCartService.GetShoppingCart();
            int count = cart.Sum(x => x.Quantity);
            return Json(new { count = count });
        }
    }
}
