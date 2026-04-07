using SV22T1020439.DataLayers.Interfaces;
using SV22T1020439.DataLayers.SQLServer;
using SV22T1020439.Models;

namespace SV22T1020439.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountRepository employeeDB;
        private static readonly IUserAccountRepository customerDB;

        static UserAccountService()
        {
            employeeDB = new EmployeeDAL(Configuration.ConnectionString);
            customerDB = new CustomerDAL(Configuration.ConnectionString);
        }

        public enum UserTypes
        {
            Employee,
            Customer
        }

        public static UserAccount? Authorize(UserTypes userType, string email, string password)
        {
            Employee? employee = null;
            if (userType == UserTypes.Employee)
                employee = employeeDB.Authorize(email, password);
            else
                employee = customerDB.Authorize(email, password);

            if (employee == null)
                return null;

            return new UserAccount()
            {
                UserId = employee.EmployeeID.ToString(),
                UserName = employee.Email,
                DisplayName = employee.FullName,
                Photo = employee.Photo,
                RoleNames = employee.RoleNames
            };
        }

        public static bool ChangePassword(UserTypes userType, string email, string password)
        {
            if (userType == UserTypes.Employee)
                return employeeDB.ChangePassword(email, password);
            else
                return customerDB.ChangePassword(email, password);
        }
    }
}
