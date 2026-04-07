using Dapper;
using SV22T1020439.Models;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using SV22T1020439.DataLayers.Interfaces;

namespace SV22T1020439.DataLayers.SQLServer
{
    public class OrderDAL : IOrderRepository
    {
        private string _connectionString;

        public OrderDAL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<RevenueByMonth> GetRevenueByMonth(int year, int months)
        {
            var list = new List<RevenueByMonth>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                    select year(o.OrderTime) as Year, month(o.OrderTime) as Month, isnull(sum(od.Quantity * od.SalePrice),0) as Revenue
                    from Orders o
                    left join OrderDetails od on o.OrderID = od.OrderID
                    where year(o.OrderTime) = @Year
                    group by year(o.OrderTime), month(o.OrderTime)
                    order by Month";

                list = connection.Query<RevenueByMonth>(sql, new { Year = year }).ToList();
            }
            return list;
        }

        public List<TopProductStat> GetTopProducts(int top)
        {
            var list = new List<TopProductStat>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                    select top(@Top) p.ProductName, sum(od.Quantity) as SoldQuantity
                    from OrderDetails od
                    join Products p on od.ProductID = p.ProductID
                    group by p.ProductName
                    order by SoldQuantity desc";

                list = connection.Query<TopProductStat>(sql, new { Top = top }).ToList();
            }
            return list;
        }

        public IList<Order> List(int page = 1, int pageSize = 0,
            int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            List<Order> list = new List<Order>();
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"with cte as
                            (
                                select  o.*,
                                        c.CustomerName,
                                        c.ContactName as CustomerContactName,
                                        c.Address as CustomerAddress,
                                        c.Phone as CustomerPhone,
                                        c.Email as CustomerEmail,
                                        e.FullName as EmployeeName,
                                        s.ShipperName,
                                        s.Phone as ShipperPhone,
                                        (select sum(Quantity * SalePrice) from OrderDetails where OrderID = o.OrderID) as TotalPrice
                                from    Orders as o
                                        left join Customers as c on o.CustomerID = c.CustomerID
                                        left join Employees as e on o.EmployeeID = e.EmployeeID
                                        left join Shippers as s on o.ShipperID = s.ShipperID
                                where   (@Status = -1 or o.Status = @Status)
                                    and (@FromTime is null or o.OrderTime >= @FromTime)
                                    and (@ToTime is null or o.OrderTime <= @ToTime)
                                    and (@SearchValue = N'' or c.CustomerName like @SearchValue or e.FullName like @SearchValue or s.ShipperName like @SearchValue)
                            )
                            select *
                            from (
                                select *, ROW_NUMBER() over (order by OrderTime desc) as RowNumber
                                from cte
                            ) as t
                            where (@PageSize = 0) or (RowNumber between (@Page - 1) * @PageSize + 1 and @Page * @PageSize)
                            order by RowNumber";
                
                var parameters = new
                {
                    Status = status,
                    FromTime = fromTime,
                    ToTime = toTime,
                    SearchValue = searchValue ?? "",
                    Page = page,
                    PageSize = pageSize
                };

                list = connection.Query<Order>(sql, parameters).ToList();
            }
            return list;
        }

        public int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select  count(*)
                            from    Orders as o
                                    left join Customers as c on o.CustomerID = c.CustomerID
                                    left join Employees as e on o.EmployeeID = e.EmployeeID
                                    left join Shippers as s on o.ShipperID = s.ShipperID
                            where   (@Status = -1 or o.Status = @Status)
                                and (@FromTime is null or o.OrderTime >= @FromTime)
                                and (@ToTime is null or o.OrderTime <= @ToTime)
                                and (@SearchValue = N'' or c.CustomerName like @SearchValue or e.FullName like @SearchValue or s.ShipperName like @SearchValue)";
                
                var parameters = new
                {
                    Status = status,
                    FromTime = fromTime,
                    ToTime = toTime,
                    SearchValue = searchValue ?? ""
                };

                count = connection.ExecuteScalar<int>(sql, parameters);
            }
            return count;
        }

        public Order? Get(int orderID)
        {
            Order? data = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select  o.*,
                                    c.CustomerName,
                                    c.ContactName as CustomerContactName,
                                    c.Address as CustomerAddress,
                                    c.Phone as CustomerPhone,
                                    c.Email as CustomerEmail,
                                    e.FullName as EmployeeName,
                                    s.ShipperName,
                                    s.Phone as ShipperPhone
                            from    Orders as o
                                    left join Customers as c on o.CustomerID = c.CustomerID
                                    left join Employees as e on o.EmployeeID = e.EmployeeID
                                    left join Shippers as s on o.ShipperID = s.ShipperID
                            where   o.OrderID = @OrderID";
                
                data = connection.QueryFirstOrDefault<Order>(sql, new { OrderID = orderID });
                if (data != null)
                {
                    data.Details = ListDetails(orderID);
                }
            }
            return data;
        }

        public List<OrderDetail> ListDetails(int orderID)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"select  od.*, p.ProductName, p.Photo, p.Unit
                            from    OrderDetails as od
                                    join Products as p on od.ProductID = p.ProductID
                            where   od.OrderID = @OrderID";
                
                list = connection.Query<OrderDetail>(sql, new { OrderID = orderID }).ToList();
            }
            return list;
        }

        public int Add(Order data)
        {
            int id = 0;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"insert into Orders(CustomerID, OrderTime, DeliveryProvince, DeliveryAddress, EmployeeID, Status)
                            values(@CustomerID, getdate(), @DeliveryProvince, @DeliveryAddress, @EmployeeID, @Status);
                            select @@IDENTITY";
                
                id = connection.ExecuteScalar<int>(sql, data);
            }
            return id;
        }

        public bool Update(Order data)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update Orders
                            set CustomerID = @CustomerID,
                                DeliveryProvince = @DeliveryProvince,
                                DeliveryAddress = @DeliveryAddress,
                                EmployeeID = @EmployeeID,
                                AcceptTime = @AcceptTime,
                                ShipperID = @ShipperID,
                                ShippedTime = @ShippedTime,
                                FinishedTime = @FinishedTime,
                                Status = @Status
                            where OrderID = @OrderID";
                
                result = connection.Execute(sql, data) > 0;
            }
            return result;
        }

        public bool Delete(int orderID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from OrderDetails where OrderID = @OrderID;
                            delete from Orders where OrderID = @OrderID";
                
                result = connection.Execute(sql, new { OrderID = orderID }) > 0;
            }
            return result;
        }

        public bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"if exists(select * from OrderDetails where OrderID = @OrderID and ProductID = @ProductID)
                                update OrderDetails
                                set Quantity = @Quantity, SalePrice = @SalePrice
                                where OrderID = @OrderID and ProductID = @ProductID
                            else
                                insert into OrderDetails(OrderID, ProductID, Quantity, SalePrice)
                                values(@OrderID, @ProductID, @Quantity, @SalePrice)";
                
                result = connection.Execute(sql, new { OrderID = orderID, ProductID = productID, Quantity = quantity, SalePrice = salePrice }) > 0;
            }
            return result;
        }

        public bool DeleteDetail(int orderID, int productID)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"delete from OrderDetails where OrderID = @OrderID and ProductID = @ProductID";
                
                result = connection.Execute(sql, new { OrderID = orderID, ProductID = productID }) > 0;
            }
            return result;
        }

        public bool UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            bool result = false;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"update OrderDetails 
                            set Quantity = @Quantity, 
                                SalePrice = @SalePrice
                            where OrderID = @OrderID and ProductID = @ProductID";
                
                result = connection.Execute(sql, new { 
                    OrderID = orderID, 
                    ProductID = productID, 
                    Quantity = quantity, 
                    SalePrice = salePrice 
                }) > 0;
            }
            return result;
        }
    }
}
