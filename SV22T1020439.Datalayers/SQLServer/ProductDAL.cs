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
    public class ProductDAL : ICommonDAL<Product>, IProductRepository
    {
        private string _connectionString;

        public ProductDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                            else
                                begin
                                    insert into Products(ProductName, ProductDescription, Unit, Price, Photo, CategoryID, SupplierID, IsSelling)
                                    values(@ProductName, @ProductDescription, @Unit, @Price, @Photo, @CategoryID, @SupplierID, @IsSelling);
                                    select @@IDENTITY;
                                end";
                id = connection.ExecuteScalar<int>(sql, data);
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select count(*) from Products 
                            where ((@searchValue = N'') or (ProductName like @searchValue))
                                and (@categoryID = 0 or CategoryID = @categoryID)
                                and (@supplierID = 0 or SupplierID = @supplierID)
                                and (Price >= @minPrice)
                                and (@maxPrice <= 0 or Price <= @maxPrice)";
                count = connection.ExecuteScalar<int>(sql, new
                {
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                });
            }
            return count;
        }

        public int Count(string searchValue = "")
        {
            return Count(searchValue, 0, 0, 0, 0);
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                result = connection.Execute(sql, new { ProductID = id }) > 0;
            }
            return result;
        }

        public Product? Get(int id)
        {
            Product? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select p.*, c.CategoryName, s.SupplierName
                            from Products p
                            left join Categories c on p.CategoryID = c.CategoryID
                            left join Suppliers s on p.SupplierID = s.SupplierID
                            where p.ProductID = @ProductID";
                data = connection.QueryFirstOrDefault<Product>(sql, new { ProductID = id });
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                result = connection.ExecuteScalar<bool>(sql, new { ProductID = id });
            }
            return result;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select *
                            from (
                                select p.*, c.CategoryName, s.SupplierName, ROW_NUMBER() over (order by p.ProductName) as RowNumber
                                from Products p
                                left join Categories c on p.CategoryID = c.CategoryID
                                left join Suppliers s on p.SupplierID = s.SupplierID
                                where ((@searchValue = N'') or (p.ProductName like @searchValue))
                                    and (@categoryID = 0 or p.CategoryID = @categoryID)
                                    and (@supplierID = 0 or p.SupplierID = @supplierID)
                                    and (p.Price >= @minPrice)
                                    and (@maxPrice <= 0 or p.Price <= @maxPrice)
                            ) as t
                            where (@pageSize = 0) 
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";
                data = connection.Query<Product>(sql, new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                }).ToList();
            }
            return data;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return List(page, pageSize, searchValue, 0, 0, 0, 0);
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                                begin
                                    update Products 
                                    set ProductName = @ProductName,
                                        ProductDescription = @ProductDescription,
                                        Unit = @Unit,
                                        Price = @Price,
                                        Photo = @Photo,
                                        CategoryID = @CategoryID,
                                        SupplierID = @SupplierID,
                                        IsSelling = @IsSelling
                                    where ProductID = @ProductID
                                end";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }

        public List<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> list = new List<ProductPhoto>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from ProductPhotos where ProductID = @ProductID order by DisplayOrder";
                list = connection.Query<ProductPhoto>(sql, new { ProductID = productID }).ToList();
            }
            return list;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql, new { PhotoID = photoID });
            }
            return data;
        }

        public long AddPhoto(ProductPhoto data)
        {
            long id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            values(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            select @@IDENTITY;";
                id = connection.ExecuteScalar<long>(sql, data);
            }
            return id;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update ProductPhotos
                            set ProductID = @ProductID,
                                Photo = @Photo,
                                Description = @Description,
                                DisplayOrder = @DisplayOrder,
                                IsHidden = @IsHidden
                            where PhotoID = @PhotoID";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                result = connection.Execute(sql, new { PhotoID = photoID }) > 0;
            }
            return result;
        }

        public List<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> list = new List<ProductAttribute>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from ProductAttributes where ProductID = @ProductID order by DisplayOrder";
                list = connection.Query<ProductAttribute>(sql, new { ProductID = productID }).ToList();
            }
            return list;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql, new { AttributeID = attributeID });
            }
            return data;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                            values(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                            select @@IDENTITY;";
                id = connection.ExecuteScalar<long>(sql, data);
            }
            return id;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update ProductAttributes
                            set ProductID = @ProductID,
                                AttributeName = @AttributeName,
                                AttributeValue = @AttributeValue,
                                DisplayOrder = @DisplayOrder
                            where AttributeID = @AttributeID";
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }

        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                result = connection.Execute(sql, new { AttributeID = attributeID }) > 0;
            }
            return result;
        }
    }
}
