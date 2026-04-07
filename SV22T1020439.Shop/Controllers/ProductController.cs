using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;

namespace SV22T1020439.Shop.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int id)
        {
            var product = ProductDataService.GetProduct(id);
            if (product == null) return RedirectToAction("Index", "Home");

            ViewBag.Photos = ProductDataService.ListPhotos(id);
            ViewBag.Attributes = ProductDataService.ListAttributes(id);
            
            return View(product);
        }
    }
}
