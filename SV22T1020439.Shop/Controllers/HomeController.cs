using Microsoft.AspNetCore.Mvc;
using SV22T1020439.Shop.Models;
using System.Diagnostics;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;

namespace SV22T1020439.Shop.Controllers
{
    public class HomeController : Controller
    {
        private const int PAGE_SIZE = 12;

        public IActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, string minPrice = "", string maxPrice = "")
        {
            decimal min = 0, max = 0;
            decimal.TryParse(minPrice?.Replace(".", "").Replace(",", ""), out min);
            decimal.TryParse(maxPrice?.Replace(".", "").Replace(",", ""), out max);

            int rowCount = ProductDataService.CountProducts(searchValue, categoryID, 0, min, max);
            var data = ProductDataService.ListOfProducts(page, PAGE_SIZE, searchValue, categoryID, 0, min, max);
            
            var model = new ProductSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue,
                CategoryID = categoryID,
                MinPrice = min,
                MaxPrice = max,
                RowCount = rowCount,
                Data = data
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
