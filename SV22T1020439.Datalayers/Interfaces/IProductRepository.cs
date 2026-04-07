using System.Collections.Generic;
using SV22T1020439.Models;

namespace SV22T1020439.DataLayers.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0);

        List<ProductPhoto> ListPhotos(int productID);
        ProductPhoto? GetPhoto(long photoID);
        long AddPhoto(ProductPhoto data);
        bool UpdatePhoto(ProductPhoto data);
        bool DeletePhoto(long photoID);

        List<ProductAttribute> ListAttributes(int productID);
        ProductAttribute? GetAttribute(long attributeID);
        long AddAttribute(ProductAttribute data);
        bool UpdateAttribute(ProductAttribute data);
        bool DeleteAttribute(long attributeID);
    }
}

