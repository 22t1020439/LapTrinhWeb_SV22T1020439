using SV22T1020439.Models;
using System.Collections.Generic;

namespace SV22T1020439.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng nghiệp vụ liên quan đến mặt hàng
    /// (Sử dụng CatalogDataService để thực hiện)
    /// </summary>
    public static class ProductDataService
    {
        public static List<Product> ListOfProducts(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            return CatalogDataService.ListOfProducts(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice);
        }

        public static int CountProducts(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            return CatalogDataService.CountProducts(searchValue, categoryID, supplierID, minPrice, maxPrice);
        }

        public static List<Product> ListOfProducts(int page = 1, int pageSize = 0, string searchValue = "")
        {
            return CatalogDataService.ListOfProducts(page, pageSize, searchValue);
        }



        public static Product? GetProduct(int id)
        {
            return CatalogDataService.GetProduct(id);
        }

        public static int AddProduct(Product data)
        {
            return CatalogDataService.AddProduct(data);
        }

        public static bool UpdateProduct(Product data)
        {
            return CatalogDataService.UpdateProduct(data);
        }

        public static bool DeleteProduct(int id)
        {
            return CatalogDataService.DeleteProduct(id);
        }

        public static bool IsUsedProduct(int id)
        {
            return CatalogDataService.IsUsedProduct(id);
        }

        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return CatalogDataService.ListPhotos(productID);
        }

        public static ProductPhoto? GetPhoto(long photoID)
        {
            return CatalogDataService.GetPhoto(photoID);
        }

        public static long AddPhoto(ProductPhoto data)
        {
            return CatalogDataService.AddPhoto(data);
        }

        public static bool UpdatePhoto(ProductPhoto data)
        {
            return CatalogDataService.UpdatePhoto(data);
        }

        public static bool DeletePhoto(long photoID)
        {
            return CatalogDataService.DeletePhoto(photoID);
        }

        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return CatalogDataService.ListAttributes(productID);
        }

        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return CatalogDataService.GetAttribute(attributeID);
        }

        public static long AddAttribute(ProductAttribute data)
        {
            return CatalogDataService.AddAttribute(data);
        }

        public static bool UpdateAttribute(ProductAttribute data)
        {
            return CatalogDataService.UpdateAttribute(data);
        }

        public static bool DeleteAttribute(long attributeID)
        {
            return CatalogDataService.DeleteAttribute(attributeID);
        }
    }
}
