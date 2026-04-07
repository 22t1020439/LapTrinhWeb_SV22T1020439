using System;
using System.Collections.Generic;
using SV22T1020439.Models;

namespace SV22T1020439.Admin.Models
{
    public class DashboardViewModel
    {
        public decimal TodayRevenue { get; set; }
        public int OrderCount { get; set; }
        public int CustomerCount { get; set; }
        public int ProductCount { get; set; }
        
        public List<MonthlyRevenue> MonthlyRevenues { get; set; } = new List<MonthlyRevenue>();
        public List<TopProduct> TopProducts { get; set; } = new List<TopProduct>();
        public List<Order> PendingOrders { get; set; } = new List<Order>();
    }

    public class MonthlyRevenue
    {
        public string MonthName { get; set; } = "";
        public decimal Revenue { get; set; }
    }

    public class TopProduct
    {
        public string ProductName { get; set; } = "";
        public int SoldQuantity { get; set; }
    }
}
