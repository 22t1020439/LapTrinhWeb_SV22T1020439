using SV22T1020439.DataLayers;
using SV22T1020439.DataLayers.SQLServer;
using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến danh mục hàng hóa của hệ thống, 
    /// bao gồm: mặt hàng (Product), thuộc tính của mặt hàng (ProductAttribute) và ảnh của mặt hàng (ProductPhoto).
    /// </summary>
    public static class CatalogDataService
    {
        private static readonly ICommonDAL<Product> productDB;
        private static readonly ICommonDAL<Category> categoryDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static CatalogDataService()
        {
            string connectionString = Configuration.ConnectionString;
            categoryDB = new CategoryDAL(connectionString);
            productDB = new ProductDAL(connectionString);
        }

        #region Category

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng dưới dạng phân trang.
        /// </summary>
        public static List<Category> ListOfCategories(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return categoryDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Đếm số lượng loại hàng tìm được.
        /// </summary>
        public static int CountCategories(string searchValue = "")
        {
            return categoryDB.Count(searchValue);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một loại hàng dựa vào mã loại hàng.
        /// </summary>
        public static Category? GetCategory(int categoryID)
        {
            return categoryDB.Get(categoryID);
        }

        /// <summary>
        /// Bổ sung một loại hàng mới vào hệ thống.
        /// </summary>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một loại hàng.
        /// </summary>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        /// <summary>
        /// Xóa một loại hàng dựa vào mã loại hàng.
        /// </summary>
        public static bool DeleteCategory(int categoryID)
        {
            if (categoryDB.InUsed(categoryID))
                return false;

            return categoryDB.Delete(categoryID);
        }

        /// <summary>
        /// Kiểm tra xem một loại hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static bool IsUsedCategory(int categoryID)
        {
            return categoryDB.InUsed(categoryID);
        }

        #endregion

        #region Product

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang.
        /// </summary>
        public static List<Product> ListOfProducts(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            return ((ProductDAL)productDB).List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice);
        }

        /// <summary>
        /// Đếm số lượng mặt hàng tìm được.
        /// </summary>
        public static int CountProducts(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            return ((ProductDAL)productDB).Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một mặt hàng.
        /// </summary>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }

        /// <summary>
        /// Bổ sung một mặt hàng mới vào hệ thống.
        /// </summary>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một mặt hàng.
        /// </summary>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }

        /// <summary>
        /// Xóa một mặt hàng dựa vào mã mặt hàng.
        /// </summary>
        public static bool DeleteProduct(int productID)
        {
            if (productDB.InUsed(productID))
                return false;

            return productDB.Delete(productID);
        }

        /// <summary>
        /// Kiểm tra xem một mặt hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static bool IsUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }

        #endregion

        #region ProductAttribute

        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return ((ProductDAL)productDB).ListAttributes(productID);
        }

        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return ((ProductDAL)productDB).GetAttribute(attributeID);
        }

        public static long AddAttribute(ProductAttribute data)
        {
            return ((ProductDAL)productDB).AddAttribute(data);
        }

        public static bool UpdateAttribute(ProductAttribute data)
        {
            return ((ProductDAL)productDB).UpdateAttribute(data);
        }

        public static bool DeleteAttribute(long attributeID)
        {
            return ((ProductDAL)productDB).DeleteAttribute(attributeID);
        }

        #endregion

        #region ProductPhoto

        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return ((ProductDAL)productDB).ListPhotos(productID);
        }

        public static ProductPhoto? GetPhoto(long photoID)
        {
            return ((ProductDAL)productDB).GetPhoto(photoID);
        }

        public static long AddPhoto(ProductPhoto data)
        {
            return ((ProductDAL)productDB).AddPhoto(data);
        }

        public static bool UpdatePhoto(ProductPhoto data)
        {
            return ((ProductDAL)productDB).UpdatePhoto(data);
        }

        public static bool DeletePhoto(long photoID)
        {
            return ((ProductDAL)productDB).DeletePhoto(photoID);
        }

        #endregion
    }
}
