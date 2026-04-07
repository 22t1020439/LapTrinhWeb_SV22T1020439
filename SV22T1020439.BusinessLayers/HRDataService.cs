using SV22T1020439.DataLayers;
using SV22T1020439.DataLayers.SQLServer;
using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến nhân sự của hệ thống    
    /// </summary>
    public static class HRDataService
    {
        private static readonly ICommonDAL<Employee> employeeDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static HRDataService()
        {
            employeeDB = new EmployeeDAL(Configuration.ConnectionString);
        }

        #region Employee

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhân viên dưới dạng phân trang.
        /// </summary>
        public static List<Employee> ListOfEmployees(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return employeeDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Đếm số lượng nhân viên tìm được.
        /// </summary>
        public static int CountEmployees(string searchValue = "")
        {
            return employeeDB.Count(searchValue);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một nhân viên dựa vào mã nhân viên.
        /// </summary>
        public static Employee? GetEmployee(int employeeID)
        {
            return employeeDB.Get(employeeID);
        }

        /// <summary>
        /// Bổ sung một nhân viên mới vào hệ thống.
        /// </summary>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một nhân viên.
        /// </summary>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }

        /// <summary>
        /// Xóa một nhân viên dựa vào mã nhân viên.
        /// </summary>
        public static bool DeleteEmployee(int employeeID)
        {
            if (employeeDB.InUsed(employeeID))
                return false;

            return employeeDB.Delete(employeeID);
        }

        /// <summary>
        /// Kiểm tra xem một nhân viên có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static bool IsUsedEmployee(int employeeID)
        {
            return employeeID > 0 && employeeDB.InUsed(employeeID);
        }

        /// <summary>
        /// Kiểm tra xem email của nhân viên đã được sử dụng hay chưa.
        /// </summary>
        public static bool ValidateEmployeeEmail(string email, int id = 0)
        {
            return ((EmployeeDAL)employeeDB).ValidateEmail(email, id);
        }

        /// <summary>
        /// Thay đổi mật khẩu của nhân viên
        /// </summary>
        public static bool ChangePassword(int id, string password)
        {
            return ((EmployeeDAL)employeeDB).ChangePassword(id, password);
        }

        /// <summary>
        /// Thay đổi quyền (vai trò) của nhân viên
        /// </summary>
        public static bool ChangeRole(int id, string roleNames)
        {
            return ((EmployeeDAL)employeeDB).ChangeRole(id, roleNames);
        }

        #endregion
    }
}
