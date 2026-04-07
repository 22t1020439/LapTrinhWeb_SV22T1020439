using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.Admin.Models;
using SV22T1020439.BusinessLayers;
using System.Diagnostics;

namespace SV22T1020439.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "Trang chủ";
            
            // Lấy dữ liệu thực tế từ các Service (Giả lập một số dữ liệu thống kê vì Service chưa hỗ trợ hết)
            var model = new DashboardViewModel();
            model.TodayRevenue = SalesDataService.GetRevenueByMonth(DateTime.Now.Year, 12)
                .Where(r => r.Year == DateTime.Now.Year && r.Month == DateTime.Now.Month)
                .Sum(r => r.Revenue);
            model.OrderCount = OrderDataService.CountOrders();
            model.CustomerCount = CommonDataService.CountCustomers();
            model.ProductCount = ProductDataService.CountProducts();

            // Monthly revenues (last 6 months)
            var revenues = SalesDataService.GetRevenueByMonth(DateTime.Now.Year, 12);
            model.MonthlyRevenues = revenues.OrderBy(r => r.Month)
                .Select(r => new MonthlyRevenue { MonthName = "Tháng " + r.Month, Revenue = r.Revenue / 1000000m })
                .ToList();

            // Top products
            model.TopProducts = SalesDataService.GetTopProducts(5)
                .Select(p => new TopProduct { ProductName = p.ProductName, SoldQuantity = p.SoldQuantity })
                .ToList();

            // Đơn hàng cần xử lý (Lấy từ DB, trạng thái 1: Chờ duyệt, 2: Đã duyệt)
            model.PendingOrders = OrderDataService.ListOrders(1, 5, 0);

            return View(model);
        }

        public IActionResult Privacy()
        {
            ViewBag.Title = "Chính sách bảo mật";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
