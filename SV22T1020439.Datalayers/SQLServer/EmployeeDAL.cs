using Dapper;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using SV22T1020439.DataLayers.Interfaces;

namespace SV22T1020439.DataLayers.SQLServer
{
    public class EmployeeDAL : ICommonDAL<Employee>, IEmployeeRepository, IUserAccountRepository
    {
        private string _connectionString;

        public EmployeeDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName, BirthDate, Address, Phone, Email, Photo, IsWorking, Password, RoleNames)
                                    values(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking, @Password, @RoleNames);
                                    select @@IDENTITY;
                                end";
                id = connection.ExecuteScalar<int>(sql, data);
            }
            return id;
        }

        public bool ChangePassword(int id, string password)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Employees set Password = @Password where EmployeeID = @EmployeeID";
                result = connection.Execute(sql, new { EmployeeID = id, Password = password }) > 0;
            }
            return result;
        }

        public bool ValidateEmail(string email, int id = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select case when exists(
                                select * from Employees 
                                where Email = @Email and (@EmployeeID = 0 or EmployeeID <> @EmployeeID)
                            ) then 0 else 1 end";
                return connection.ExecuteScalar<bool>(sql, new { Email = email ?? "", EmployeeID = id });
            }
        }

        public Employee? Authorize(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from Employees 
                            where RTRIM(Email) = @Email 
                              and RTRIM(Password) = @Password 
                              and IsWorking = 1";
                return connection.QueryFirstOrDefault<Employee>(sql, new { Email = (email ?? "").Trim(), Password = (password ?? "").Trim() });
            }
        }

        bool IUserAccountRepository.ChangePassword(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Employees set Password = @Password where Email = @Email";
                return connection.Execute(sql, new { Email = email ?? "", Password = password ?? "" }) > 0;
            }
        }

        public bool ChangeRole(int id, string roleNames)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Employees set RoleNames = @RoleNames where EmployeeID = @EmployeeID";
                result = connection.Execute(sql, new { EmployeeID = id, RoleNames = roleNames }) > 0;
            }
            return result;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select count(*) from Employees 
                            where (@searchValue = N'') 
                                or (FullName like @searchValue)
                                or (Address like @searchValue)
                                or (Email like @searchValue)";
                count = connection.ExecuteScalar<int>(sql, new { searchValue = searchValue ?? "" });
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from Employees where EmployeeID = @EmployeeID";
                result = connection.Execute(sql, new { EmployeeID = id }) > 0;
            }
            return result;
        }

        public Employee? Get(int id)
        {
            Employee? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from Employees where EmployeeID = @EmployeeID";
                data = connection.QueryFirstOrDefault<Employee>(sql, new { EmployeeID = id });
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Orders where EmployeeID = @EmployeeID)
                                select 1
                            else 
                                select 0";
                result = connection.ExecuteScalar<bool>(sql, new { EmployeeID = id });
            }
            return result;
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from (
                                select *, ROW_NUMBER() over (order by FullName) as RowNumber
                                from Employees
                                where (@searchValue = N'') 
                                    or (FullName like @searchValue)
                                    or (Address like @searchValue)
                                    or (Email like @searchValue)
                            ) as t
                            where (@pageSize = 0) 
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";
                data = connection.Query<Employee>(sql, new { page = page, pageSize = pageSize, searchValue = searchValue ?? "" }).ToList();
            }
            return data;
        }

        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if not exists(select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                                begin
                                    update Employees 
                                    set FullName = @FullName,
                                        BirthDate = @BirthDate,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        Photo = @Photo,
                                        IsWorking = @IsWorking
                                    where EmployeeID = @EmployeeID
                                end";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }
    }
}
