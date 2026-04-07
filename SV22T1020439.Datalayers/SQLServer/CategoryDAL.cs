using Dapper;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace SV22T1020439.DataLayers.SQLServer
{
    public class CategoryDAL : ICommonDAL<Category>
    {
        private string _connectionString;

        public CategoryDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Category data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Categories where CategoryName = @CategoryName)
                                select -1
                            else
                                begin
                                    insert into Categories(CategoryName, Description)
                                    values(@CategoryName, @Description);
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
                var sql = @"select count(*) from Categories 
                            where (@searchValue = N'') or (CategoryName like @searchValue)";
                count = connection.ExecuteScalar<int>(sql, new { searchValue = searchValue ?? "" });
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from Categories where CategoryID = @CategoryID";
                result = connection.Execute(sql, new { CategoryID = id }) > 0;
            }
            return result;
        }

        public Category? Get(int id)
        {
            Category? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from Categories where CategoryID = @CategoryID";
                data = connection.QueryFirstOrDefault<Category>(sql, new { CategoryID = id });
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Products where CategoryID = @CategoryID)
                                select 1
                            else 
                                select 0";
                result = connection.ExecuteScalar<bool>(sql, new { CategoryID = id });
            }
            return result;
        }

        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Category> data = new List<Category>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from (
                                select *, ROW_NUMBER() over (order by CategoryName) as RowNumber
                                from Categories
                                where (@searchValue = N'') or (CategoryName like @searchValue)
                            ) as t
                            where (@pageSize = 0) 
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";
                data = connection.Query<Category>(sql, new { page = page, pageSize = pageSize, searchValue = searchValue ?? "" }).ToList();
            }
            return data;
        }

        public bool Update(Category data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if not exists(select * from Categories where CategoryID <> @CategoryID and CategoryName = @CategoryName)
                                begin
                                    update Categories 
                                    set CategoryName = @CategoryName,
                                        Description = @Description
                                    where CategoryID = @CategoryID
                                end";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }
    }
}
