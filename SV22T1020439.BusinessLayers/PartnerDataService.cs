using SV22T1020439.DataLayers;
using SV22T1020439.DataLayers.SQLServer;
using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến các đối tác của hệ thống
    /// bao gồm: nhà cung cấp (Supplier), khách hàng (Customer) và người giao hàng (Shipper)
    /// </summary>
    public static class PartnerDataService
    {
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static PartnerDataService()
        {
            string connectionString = Configuration.ConnectionString;
            supplierDB = new SupplierDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
        }

        #region Supplier

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhà cung cấp dưới dạng phân trang.
        /// </summary>
        public static List<Supplier> ListOfSuppliers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return supplierDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Đếm số lượng nhà cung cấp tìm được.
        /// </summary>
        public static int CountSuppliers(string searchValue = "")
        {
            return supplierDB.Count(searchValue);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhà cung cấp dựa vào mã nhà cung cấp.
        /// </summary>
        public static Supplier? GetSupplier(int supplierID)
        {
            return supplierDB.Get(supplierID);
        }

        /// <summary>
        /// Bổ sung một nhà cung cấp mới vào hệ thống.
        /// </summary>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhà cung cấp.
        /// </summary>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }

        /// <summary>
        /// Xóa một nhà cung cấp dựa vào mã nhà cung cấp.
        /// </summary>
        public static bool DeleteSupplier(int supplierID)
        {
            if (supplierDB.InUsed(supplierID))
                return false;

            return supplierDB.Delete(supplierID);
        }

        /// <summary>
        /// Kiểm tra xem một nhà cung cấp có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static bool IsUsedSupplier(int supplierID)
        {
            return supplierDB.InUsed(supplierID);
        }

        #endregion

        #region Customer

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang.
        /// </summary>
        public static List<Customer> ListOfCustomers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return customerDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Đếm số lượng khách hàng tìm được.
        /// </summary>
        public static int CountCustomers(string searchValue = "")
        {
            return customerDB.Count(searchValue);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một khách hàng dựa vào mã khách hàng.
        /// </summary>
        public static Customer? GetCustomer(int customerID)
        {
            return customerDB.Get(customerID);
        }

        /// <summary>
        /// Bổ sung một khách hàng mới vào hệ thống.
        /// </summary>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một khách hàng.
        /// </summary>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }

        /// <summary>
        /// Xóa một khách hàng dựa vào mã khách hàng.
        /// </summary>
        public static bool DeleteCustomer(int customerID)
        {
            if (customerDB.InUsed(customerID))
                return false;

            return customerDB.Delete(customerID);
        }

        /// <summary>
        /// Kiểm tra xem một khách hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static bool IsUsedCustomer(int customerID)
        {
            return customerDB.InUsed(customerID);
        }

        /// <summary>
        /// Thay đổi mật khẩu cho khách hàng
        /// </summary>
        public static bool ChangePassword(int id, string password)
        {
            return ((CustomerDAL)customerDB).ChangePassword(id, password);
        }

        #endregion

        #region Shipper

        /// <summary>
        /// Tìm kiếm và lấy danh sách người giao hàng dưới dạng phân trang.
        /// </summary>
        public static List<Shipper> ListOfShippers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return shipperDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Đếm số lượng người giao hàng tìm được.
        /// </summary>
        public static int CountShippers(string searchValue = "")
        {
            return shipperDB.Count(searchValue);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người giao hàng dựa vào mã người giao hàng.
        /// </summary>
        public static Shipper? GetShipper(int shipperID)
        {
            return shipperDB.Get(shipperID);
        }

        /// <summary>
        /// Bổ sung một người giao hàng mới vào hệ thống.
        /// </summary>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một người giao hàng.
        /// </summary>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }

        /// <summary>
        /// Xóa một người giao hàng dựa vào mã người giao hàng.
        /// </summary>
        public static bool DeleteShipper(int shipperID)
        {
            if (shipperDB.InUsed(shipperID))
                return false;

            return shipperDB.Delete(shipperID);
        }

        /// <summary>
        /// Kiểm tra xem một người giao hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static bool IsUsedShipper(int shipperID)
        {
            return shipperDB.InUsed(shipperID);
        }

        #endregion
    }
}
