using SV22T1020439.DataLayers;
using SV22T1020439.DataLayers.SQLServer;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến bán hàng
    /// bao gồm: đơn hàng (Order) và chi tiết đơn hàng (OrderDetail).
    /// </summary>
    public static class SalesDataService
    {
        private static readonly OrderDAL orderDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static SalesDataService()
        {
            orderDB = new OrderDAL(Configuration.ConnectionString);
        }

        #region Order

        /// <summary>
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng phân trang
        /// </summary>
        public static List<Order> ListOrders(int page = 1, int pageSize = 0,
            int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            return (List<Order>)orderDB.List(page, pageSize, status, fromTime, toTime, searchValue);
        }

        /// <summary>
        /// Đếm số lượng đơn hàng tìm được
        /// </summary>
        public static int CountOrders(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            return orderDB.Count(status, fromTime, toTime, searchValue);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một đơn hàng
        /// </summary>
        public static Order? GetOrder(int orderID)
        {
            return orderDB.Get(orderID);
        }

        /// <summary>
        /// Tạo đơn hàng mới (không có chi tiết)
        /// </summary>
        public static int AddOrder(Order data)
        {
            return orderDB.Add(data);
        }

        /// <summary>
        /// Khởi tạo đơn hàng (có chi tiết)
        /// </summary>
        public static int InitOrder(int? employeeID, int? customerID, string deliveryProvince, string deliveryAddress, IEnumerable<OrderDetail> details)
        {
            if (details == null || System.Linq.Enumerable.Count(details) == 0)
                return InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress);

            Order order = new Order()
            {
                EmployeeID = employeeID,
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                Status = 0 // Đơn mới
            };

            int orderID = orderDB.Add(order);
            if (orderID > 0)
            {
                foreach (var item in details)
                {
                    orderDB.SaveDetail(orderID, item.ProductID, item.Quantity, item.SalePrice);
                }
                return orderID;
            }
            return 0;
        }

        /// <summary>
        /// Khởi tạo đơn hàng (không có chi tiết)
        /// </summary>
        public static int InitOrder(int? employeeID, int? customerID, string deliveryProvince = "", string deliveryAddress = "")
        {
            Order order = new Order()
            {
                EmployeeID = employeeID,
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                Status = 0 // Đơn mới
            };
            return orderDB.Add(order);
        }

        /// <summary>
        /// Cập nhật thông tin đơn hàng
        /// </summary>
        public static bool UpdateOrder(Order data)
        {
            return orderDB.Update(data);
        }

        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        public static bool DeleteOrder(int orderID)
        {
            return orderDB.Delete(orderID);
        }

        #endregion

        #region Analytics

        public static List<RevenueByMonth> GetRevenueByMonth(int year, int months = 6)
        {
            var db = new OrderDAL(Configuration.ConnectionString);
            return db.GetRevenueByMonth(year, months);
        }

        public static List<TopProductStat> GetTopProducts(int top = 5)
        {
            var db = new OrderDAL(Configuration.ConnectionString);
            return db.GetTopProducts(top);
        }

        #endregion

        #region Order Status Processing

        public static bool AcceptOrder(int orderID)
        {
            var order = orderDB.Get(orderID);
            if (order == null || order.Status != 0) // Chỉ duyệt đơn hàng mới (status = 0)
                return false;

            order.Status = 1; // Đã duyệt
            order.AcceptTime = DateTime.Now;
            return orderDB.Update(order);
        }

        
        public static bool CancelOrder(int orderID)
        {
            var order = orderDB.Get(orderID);
            if (order == null || order.Status == 3 || order.Status == 4)
                return false;

            order.Status = 4; // Đã hủy
            order.FinishedTime = DateTime.Now;
            return orderDB.Update(order);
        }

        public static bool ShipOrder(int orderID, int shipperID, DateTime shippedTime)
        {
            var order = orderDB.Get(orderID);
            if (order == null || order.Status != 1) // Chỉ giao hàng đơn hàng đã duyệt (status = 1)
                return false;

            order.Status = 2; // Đang giao
            order.ShipperID = shipperID;
            order.ShippedTime = shippedTime;
            return orderDB.Update(order);
        }

        public static bool FinishOrder(int orderID)
        {
            var order = orderDB.Get(orderID);
            if (order == null || order.Status != 2) // Chỉ hoàn tất đơn hàng đang giao (status = 2)
                return false;

            order.Status = 3; // Hoàn thành
            order.FinishedTime = DateTime.Now;
            return orderDB.Update(order);
        }

        public static bool RejectOrder(int orderID)
        {
            var order = orderDB.Get(orderID);
            if (order == null || order.Status != 2) // Chỉ từ chối đơn hàng đang giao (status = 2)
                return false;

            order.Status = 5; // Từ chối
            order.FinishedTime = DateTime.Now;
            return orderDB.Update(order);
        }

        #endregion

        #region Order Detail

        public static List<OrderDetail> ListDetails(int orderID)
        {
            return (List<OrderDetail>)orderDB.ListDetails(orderID);
        }

        public static OrderDetail? GetDetail(int orderID, int productID)
        {
            return orderDB.ListDetails(orderID).Find(x => x.ProductID == productID);
        }

        public static bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            return orderDB.SaveDetail(orderID, productID, quantity, salePrice);
        }

        public static bool UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            return orderDB.UpdateDetail(orderID, productID, quantity, salePrice);
        }

        public static bool DeleteDetail(int orderID, int productID)
        {
            return orderDB.DeleteDetail(orderID, productID);
        }

        #endregion
    }
}
