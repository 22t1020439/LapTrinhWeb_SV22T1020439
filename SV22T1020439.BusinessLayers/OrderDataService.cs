using SV22T1020439.Models;
using System;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng nghiệp vụ liên quan đến bán hàng
    /// (Sử dụng SalesDataService để thực hiện)
    /// </summary>
    public static class OrderDataService
    {
        public static List<Order> ListOrders(int page = 1, int pageSize = 0,
            int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            return SalesDataService.ListOrders(page, pageSize, status, fromTime, toTime, searchValue);
        }

        public static int CountOrders(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            return SalesDataService.CountOrders(status, fromTime, toTime, searchValue);
        }

        public static Order? GetOrder(int id)
        {
            return SalesDataService.GetOrder(id);
        }

        public static int InitOrder(int? employeeID, int? customerID, string deliveryProvince, string deliveryAddress, IEnumerable<OrderDetail> details)
        {
            return SalesDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, details);
        }
        
        public static int InitOrder(int? employeeID, int? customerID, string deliveryProvince = "", string deliveryAddress = "")
        {
            return SalesDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress);
        }

        public static bool UpdateOrder(Order data)
        {
            return SalesDataService.UpdateOrder(data);
        }

        public static bool CancelOrder(int id)
        {
            return SalesDataService.CancelOrder(id);
        }

        
        public static bool AcceptOrder(int id)
        {
            return SalesDataService.AcceptOrder(id);
        }

        public static bool ShipOrder(int id, int shipperID, DateTime shippedTime)
        {
            return SalesDataService.ShipOrder(id, shipperID, shippedTime);
        }

        public static bool FinishOrder(int id)
        {
            return SalesDataService.FinishOrder(id);
        }

        public static bool RejectOrder(int id)
        {
            return SalesDataService.RejectOrder(id);
        }

        public static bool DeleteOrder(int id)
        {
            return SalesDataService.DeleteOrder(id);
        }

        public static List<OrderDetail> ListDetails(int id)
        {
            return SalesDataService.ListDetails(id);
        }

        public static OrderDetail? GetOrderDetail(int id, int productID)
        {
            return SalesDataService.GetDetail(id, productID);
        }

        public static bool SaveOrderDetail(int id, int productID, int quantity, decimal salePrice)
        {
            return SalesDataService.SaveDetail(id, productID, quantity, salePrice);
        }

        public static bool UpdateOrderDetail(int id, int productID, int quantity, decimal salePrice)
        {
            return SalesDataService.UpdateDetail(id, productID, quantity, salePrice);
        }

        public static bool DeleteOrderDetail(int id, int productID)
        {
            return SalesDataService.DeleteDetail(id, productID);
        }
    }
}
