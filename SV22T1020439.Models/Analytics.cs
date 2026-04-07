using System;
using System.Collections.Generic;

namespace SV22T1020439.Models
{
    public class RevenueByMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopProductStat
    {
        public string ProductName { get; set; } = "";
        public int SoldQuantity { get; set; }
    }
}
