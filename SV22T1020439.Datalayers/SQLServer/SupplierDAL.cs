using Dapper;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace SV22T1020439.DataLayers.SQLServer
{
    public class SupplierDAL : ICommonDAL<Supplier>
    {
        private string _connectionString;

        public SupplierDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Suppliers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Suppliers(SupplierName, ContactName, Province, Address, Phone, Email)
                                    values(@SupplierName, @ContactName, @Province, @Address, @Phone, @Email);
                                    select @@IDENTITY;
                                end";
                id = connection.ExecuteScalar<int>(sql, data);
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select count(*) from Suppliers 
                            where (@searchValue = N'') 
                                or (SupplierName like @searchValue)
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
                var sql = @"delete from Suppliers where SupplierID = @SupplierID";
                result = connection.Execute(sql, new { SupplierID = id }) > 0;
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                data = connection.QueryFirstOrDefault<Supplier>(sql, new { SupplierID = id });
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Products where SupplierID = @SupplierID)
                                select 1
                            else 
                                select 0";
                result = connection.ExecuteScalar<bool>(sql, new { SupplierID = id });
            }
            return result;
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from (
                                select *, ROW_NUMBER() over (order by SupplierName) as RowNumber
                                from Suppliers
                                where (@searchValue = N'') 
                                    or (SupplierName like @searchValue)
                                    or (ContactName like @searchValue)
                                    or (Address like @searchValue)
                            ) as t
                            where (@pageSize = 0) 
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";
                data = connection.Query<Supplier>(sql, new { page = page, pageSize = pageSize, searchValue = searchValue ?? "" }).ToList();
            }
            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if not exists(select * from Suppliers where SupplierID <> @SupplierID and Email = @Email)
                                begin
                                    update Suppliers 
                                    set SupplierName = @SupplierName,
                                        ContactName = @ContactName,
                                        Province = @Province,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email
                                    where SupplierID = @SupplierID
                                end";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }
    }
}
