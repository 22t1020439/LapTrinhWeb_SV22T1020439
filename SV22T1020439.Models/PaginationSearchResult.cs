using System.Collections.Generic;

namespace SV22T1020439.Models
{
    /// <summary>
    /// Đầu vào tìm kiếm dữ liệu để phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }

    /// <summary>
    /// Kết quả tìm kiếm phân trang
    /// </summary>
    public class PaginationSearchResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int RowCount { get; set; }
        public int PageCount 
        {
            get
            {
                if (PageSize == 0) return 1;
                int p = RowCount / PageSize;
                if (RowCount % PageSize > 0) p += 1;
                return p;
            }
        }
    }

    public class PaginationSearchResult<T> : PaginationSearchResult
    {
        public List<T> Data { get; set; } = new List<T>();
    }

    public class ProductSearchResult : PaginationSearchResult<Product>
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
    }

    public class OrderSearchResult : PaginationSearchResult<Order>
    {
        public int Status { get; set; } = 0;
        public string FromTime { get; set; } = "";
        public string ToTime { get; set; } = "";
    }

    /// <summary>
    /// Thông tin tài khoản người dùng
    /// </summary>
    public class UserAccount
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Photo { get; set; } = "";
        public string RoleNames { get; set; } = "";
    }
}
