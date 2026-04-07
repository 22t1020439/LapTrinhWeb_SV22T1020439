using Dapper;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace SV22T1020439.DataLayers.SQLServer
{
    public class ShipperDAL : ICommonDAL<Shipper>
    {
        private string _connectionString;

        public ShipperDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Shippers where Phone = @Phone)
                                select -1
                            else
                                begin
                                    insert into Shippers(ShipperName, Phone)
                                    values(@ShipperName, @Phone);
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
                var sql = @"select count(*) from Shippers 
                            where (@searchValue = N'') 
                                or (ShipperName like @searchValue)";
                count = connection.ExecuteScalar<int>(sql, new { searchValue = searchValue ?? "" });
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from Shippers where ShipperID = @ShipperID";
                result = connection.Execute(sql, new { ShipperID = id }) > 0;
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from Shippers where ShipperID = @ShipperID";
                data = connection.QueryFirstOrDefault<Shipper>(sql, new { ShipperID = id });
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Orders where ShipperID = @ShipperID)
                                select 1
                            else 
                                select 0";
                result = connection.ExecuteScalar<bool>(sql, new { ShipperID = id });
            }
            return result;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from (
                                select *, ROW_NUMBER() over (order by ShipperName) as RowNumber
                                from Shippers
                                where (@searchValue = N'') 
                                    or (ShipperName like @searchValue)
                            ) as t
                            where (@pageSize = 0) 
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";
                data = connection.Query<Shipper>(sql, new { page = page, pageSize = pageSize, searchValue = searchValue ?? "" }).ToList();
            }
            return data;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if not exists(select * from Shippers where ShipperID <> @ShipperID and Phone = @Phone)
                                begin
                                    update Shippers 
                                    set ShipperName = @ShipperName,
                                        Phone = @Phone
                                    where ShipperID = @ShipperID
                                end";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }
    }
}
