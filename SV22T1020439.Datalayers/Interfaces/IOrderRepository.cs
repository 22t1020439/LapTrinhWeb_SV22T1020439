using System;
using System.Collections.Generic;
using SV22T1020439.Models;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface IOrderRepository
    {
        IList<Order> List(int page = 1, int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "");
        int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "");
        Order? Get(int orderID);

        int Add(Order data);
        bool Update(Order data);
        bool Delete(int orderID);

        List<OrderDetail> ListDetails(int orderID);
        bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice);
        bool UpdateDetail(int orderID, int productID, int quantity, decimal salePrice);
        bool DeleteDetail(int orderID, int productID);
    }
}

