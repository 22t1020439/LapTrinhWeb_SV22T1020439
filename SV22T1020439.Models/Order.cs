using System;
using System.Collections.Generic;

namespace SV22T1020439.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int? CustomerID { get; set; }
        public DateTime OrderTime { get; set; }
        public string? DeliveryProvince { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? EmployeeID { get; set; }
        public DateTime? AcceptTime { get; set; }
        public int? ShipperID { get; set; }
        public DateTime? ShippedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public int Status { get; set; }

        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case 0: return "Đơn mới";
                    case 1: return "Đã duyệt";
                    case 2: return "Đang giao";
                    case 3: return "Hoàn thành";
                    case 4: return "Đã hủy";
                    default: return "";
                }
            }
        }

        public string CustomerName { get; set; } = "";
        public string CustomerContactName { get; set; } = "";
        public string CustomerAddress { get; set; } = "";
        public string CustomerPhone { get; set; } = "";
        public string CustomerEmail { get; set; } = "";

        public string EmployeeName { get; set; } = "";
        public string ShipperName { get; set; } = "";
        public string ShipperPhone { get; set; } = "";

        public List<OrderDetail> Details { get; set; } = new List<OrderDetail>();

        private decimal _totalPrice = 0;
        public decimal TotalPrice
        {
            get
            {
                if (Details != null && Details.Count > 0)
                {
                    decimal total = 0;
                    foreach (var detail in Details)
                    {
                        total += detail.TotalPrice;
                    }
                    return total;
                }
                return _totalPrice;
            }
            set
            {
                _totalPrice = value;
            }
        }
    }

    public class OrderDetail
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string Unit { get; set; } = "";
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice => Quantity * SalePrice;
    }

    public class OrderStatus
    {
        public int Status { get; set; }
        public string Description { get; set; } = "";
    }
}
