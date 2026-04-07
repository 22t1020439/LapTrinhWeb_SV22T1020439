using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng nghiệp vụ liên quan đến các dữ liệu chung
    /// (Tỉnh/thành, Khách hàng, Nhà cung cấp, Loại hàng, Người giao hàng, Nhân viên)
    /// (Sử dụng các DataService chuyên biệt để thực hiện)
    /// </summary>
    public static class CommonDataService
    {
        #region Province
        public static List<Province> ListOfProvinces()
        {
            return DictionaryDataService.ListOfProvinces();
        }
        #endregion

        #region Category
        public static List<Category> ListOfCategories(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return CatalogDataService.ListOfCategories(page, pageSize, searchValue);
        }
        public static int CountCategories(string searchValue = "")
        {
            return CatalogDataService.CountCategories(searchValue);
        }
        public static Category? GetCategory(int id)
        {
            return CatalogDataService.GetCategory(id);
        }
        public static int AddCategory(Category data)
        {
            return CatalogDataService.AddCategory(data);
        }
        public static bool UpdateCategory(Category data)
        {
            return CatalogDataService.UpdateCategory(data);
        }
        public static bool DeleteCategory(int id)
        {
            return CatalogDataService.DeleteCategory(id);
        }
        public static bool IsUsedCategory(int id)
        {
            return CatalogDataService.IsUsedCategory(id);
        }
        #endregion

        #region Customer
        public static List<Customer> ListOfCustomers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return PartnerDataService.ListOfCustomers(page, pageSize, searchValue);
        }
        public static int CountCustomers(string searchValue = "")
        {
            return PartnerDataService.CountCustomers(searchValue);
        }
        public static Customer? GetCustomer(int id)
        {
            return PartnerDataService.GetCustomer(id);
        }
        public static int AddCustomer(Customer data)
        {
            return PartnerDataService.AddCustomer(data);
        }
        public static bool UpdateCustomer(Customer data)
        {
            return PartnerDataService.UpdateCustomer(data);
        }
        public static bool DeleteCustomer(int id)
        {
            return PartnerDataService.DeleteCustomer(id);
        }
        public static bool IsUsedCustomer(int id)
        {
            return PartnerDataService.IsUsedCustomer(id);
        }
        public static bool ChangeCustomerPassword(int id, string password)
        {
            return PartnerDataService.ChangePassword(id, password);
        }
        #endregion

        #region Supplier
        public static List<Supplier> ListOfSuppliers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return PartnerDataService.ListOfSuppliers(page, pageSize, searchValue);
        }
        public static int CountSuppliers(string searchValue = "")
        {
            return PartnerDataService.CountSuppliers(searchValue);
        }
        public static Supplier? GetSupplier(int id)
        {
            return PartnerDataService.GetSupplier(id);
        }
        public static int AddSupplier(Supplier data)
        {
            return PartnerDataService.AddSupplier(data);
        }
        public static bool UpdateSupplier(Supplier data)
        {
            return PartnerDataService.UpdateSupplier(data);
        }
        public static bool DeleteSupplier(int id)
        {
            return PartnerDataService.DeleteSupplier(id);
        }
        public static bool IsUsedSupplier(int id)
        {
            return PartnerDataService.IsUsedSupplier(id);
        }
        #endregion

        #region Shipper
        public static List<Shipper> ListOfShippers(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return PartnerDataService.ListOfShippers(page, pageSize, searchValue);
        }
        public static int CountShippers(string searchValue = "")
        {
            return PartnerDataService.CountShippers(searchValue);
        }
        public static Shipper? GetShipper(int id)
        {
            return PartnerDataService.GetShipper(id);
        }
        public static int AddShipper(Shipper data)
        {
            return PartnerDataService.AddShipper(data);
        }
        public static bool UpdateShipper(Shipper data)
        {
            return PartnerDataService.UpdateShipper(data);
        }
        public static bool DeleteShipper(int id)
        {
            return PartnerDataService.DeleteShipper(id);
        }
        public static bool IsUsedShipper(int id)
        {
            return PartnerDataService.IsUsedShipper(id);
        }
        #endregion

        #region Employee
        public static List<Employee> ListOfEmployees(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return HRDataService.ListOfEmployees(page, pageSize, searchValue);
        }
        public static int CountEmployees(string searchValue = "")
        {
            return HRDataService.CountEmployees(searchValue);
        }
        public static Employee? GetEmployee(int id)
        {
            return HRDataService.GetEmployee(id);
        }
        public static int AddEmployee(Employee data)
        {
            return HRDataService.AddEmployee(data);
        }
        public static bool UpdateEmployee(Employee data)
        {
            return HRDataService.UpdateEmployee(data);
        }
        public static bool DeleteEmployee(int id)
        {
            return HRDataService.DeleteEmployee(id);
        }
        public static bool IsUsedEmployee(int id)
        {
            return HRDataService.IsUsedEmployee(id);
        }
        public static bool ValidateEmployeeEmail(string email, int id = 0)
        {
            return HRDataService.ValidateEmployeeEmail(email, id);
        }
        public static bool ChangeEmployeePassword(int id, string password)
        {
            return HRDataService.ChangePassword(id, password);
        }
        public static bool ChangeEmployeeRole(int id, string roleNames)
        {
            return HRDataService.ChangeRole(id, roleNames);
        }
        #endregion
    }
}
