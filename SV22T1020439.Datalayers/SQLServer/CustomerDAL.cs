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
    public class CustomerDAL : ICommonDAL<Customer>, ICustomerRepository, IUserAccountRepository
    {
        private string _connectionString;

        public CustomerDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked, Password)
                                    values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked, @Password);
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
                var sql = @"update Customers set Password = @Password where CustomerID = @CustomerID";
                result = connection.Execute(sql, new { CustomerID = id, Password = password }) > 0;
            }
            return result;
        }

        public bool ValidateEmail(string email, int id = 0)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select case when exists(
                                select * from Customers 
                                where Email = @Email and (@CustomerID = 0 or CustomerID <> @CustomerID)
                            ) then 0 else 1 end";
                return connection.ExecuteScalar<bool>(sql, new { Email = email ?? "", CustomerID = id });
            }
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select count(*) from Customers 
                            where (@searchValue = N'') 
                                or (CustomerName like @searchValue)
                                or (ContactName like @searchValue)
                                or (Address like @searchValue)";
                count = connection.ExecuteScalar<int>(sql, new { searchValue = searchValue ?? "" });
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from Customers where CustomerID = @CustomerID";
                result = connection.Execute(sql, new { CustomerID = id }) > 0;
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from Customers where CustomerID = @CustomerID";
                data = connection.QueryFirstOrDefault<Customer>(sql, new { CustomerID = id });
            }
            return data;
        }

        public Employee? Authorize(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select CustomerID as EmployeeID, CustomerName as FullName, Email, '' as Photo, Password, 
                                   case when IsLocked = 1 then 0 else 1 end as IsWorking, 
                                   'customer' as RoleNames 
                            from Customers 
                            where Email = @Email and Password = @Password and IsLocked = 0";
                return connection.QueryFirstOrDefault<Employee>(sql, new { Email = email ?? "", Password = password ?? "" });
            }
        }

        bool IUserAccountRepository.ChangePassword(string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Customers set Password = @Password where Email = @Email";
                return connection.Execute(sql, new { Email = email ?? "", Password = password ?? "" }) > 0;
            }
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Orders where CustomerID = @CustomerID)
                                select 1
                            else 
                                select 0";
                result = connection.ExecuteScalar<bool>(sql, new { CustomerID = id });
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from (
                                select *, ROW_NUMBER() over (order by CustomerName) as RowNumber
                                from Customers
                                where (@searchValue = N'') 
                                    or (CustomerName like @searchValue)
                                    or (ContactName like @searchValue)
                                    or (Address like @searchValue)
                            ) as t
                            where (@pageSize = 0) 
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";
                data = connection.Query<Customer>(sql, new { page = page, pageSize = pageSize, searchValue = searchValue ?? "" }).ToList();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @Email)
                                begin
                                    update Customers 
                                    set CustomerName = @CustomerName,
                                        ContactName = @ContactName,
                                        Province = @Province,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        IsLocked = @IsLocked
                                    where CustomerID = @CustomerID
                                end";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }
    }
}
