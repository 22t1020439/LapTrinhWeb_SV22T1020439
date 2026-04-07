using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV22T1020439.BusinessLayers;
using SV22T1020439.Models;

namespace SV22T1020439.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMIN},{WebUserRoles.PRODUCT}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;

        public IActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int rowCount = ProductDataService.CountProducts(searchValue ?? "", categoryID, supplierID, minPrice, maxPrice);
            var data = ProductDataService.ListOfProducts(page, PAGE_SIZE, searchValue ?? "", categoryID, supplierID, minPrice, maxPrice);
            var model = new ProductSearchResult()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                CategoryID = categoryID,
                SupplierID = supplierID,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                RowCount = rowCount,
                Data = data
            };
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var model = new Product()
            {
                ProductID = 0,
                Photo = "default.png",
                IsSelling = true
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult Save(Product data, string _Price, IFormFile? uploadPhoto)
        {
            decimal price = 0;
            if (!string.IsNullOrEmpty(_Price))
            {
                decimal.TryParse(_Price.Replace(".", "").Replace(",", ""), out price);
            }
            data.Price = price;

            // Kiểm tra xem đây là sản phẩm mới hay cập nhật
            bool isNewProduct = data.ProductID == 0;

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                ModelState.AddModelError(nameof(data.Unit), "Đơn tính không được để trống");
            if (data.Price <= 0)
                ModelState.AddModelError(nameof(data.Price), "Giá hàng phải lớn hơn 0");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products"); //đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
            {
                data.ProductID = ProductDataService.AddProduct(data);
            }
            else
            {
                ProductDataService.UpdateProduct(data);
            }

            // Nếu chưa có ảnh đại diện, tự động lấy ảnh đầu tiên trong thư viện
            if (string.IsNullOrWhiteSpace(data.Photo))
            {
                var photos = ProductDataService.ListPhotos(data.ProductID);
                if (photos != null && photos.Count > 0)
                {
                    data.Photo = photos[0].Photo;
                    ProductDataService.UpdateProduct(data);
                }
            }

            // Thông báo thành công
            if (isNewProduct)
            {
                TempData["Message"] = $"Thêm mặt hàng mới thành công: {data.ProductName}";
                TempData["NewProductID"] = data.ProductID; // Để highlight sản phẩm mới
            }
            else
            {
                TempData["Message"] = "Cập nhật thông tin mặt hàng thành công!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool isUsed = ProductDataService.IsUsedProduct(id);
                if (isUsed)
                {
                    TempData["ErrorMessage"] = "Không thể xóa mặt hàng này vì đã có dữ liệu liên quan (đơn hàng).";
                    return RedirectToAction("Delete", new { id = id });
                }

                ProductDataService.DeleteProduct(id);
                TempData["Message"] = "Xóa mặt hàng thành công!";
                return RedirectToAction("Index");
            }
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            
            ViewBag.IsUsed = ProductDataService.IsUsedProduct(id);
            return View(model);
        }

        public IActionResult Detail(int id = 0)
        {
            var model = ProductDataService.GetProduct(id);
            if (model == null)
                return RedirectToAction("Index");
            
            ViewBag.Photos = ProductDataService.ListPhotos(id);
            ViewBag.Attributes = ProductDataService.ListAttributes(id);
            return View(model);
        }

        public IActionResult EditPhoto(int id, long photoId = 0)
        {
            ViewBag.Title = "Thay đổi ảnh của mặt hàng";
            var model = ProductDataService.GetPhoto(photoId);
            if (model == null)
                model = new ProductPhoto { ProductID = id, Photo = "default.png", IsHidden = false, DisplayOrder = 1 };
            
            return View(model);
        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = "Thay đổi ảnh của mặt hàng";
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải > 0");

            if (!ModelState.IsValid)
            {
                return View("EditPhoto", data);
            }
            
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.PhotoID == 0)
                ProductDataService.AddPhoto(data);
            else
                ProductDataService.UpdatePhoto(data);

            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        public IActionResult DeletePhoto(int id, long photoId = 0)
        {
            ProductDataService.DeletePhoto(photoId);
            return RedirectToAction("Edit", new { id = id });
        }

        public IActionResult EditAttribute(int id, long attributeId = 0)
        {
            ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
            var model = ProductDataService.GetAttribute(attributeId);
            if (model == null)
                model = new ProductAttribute { ProductID = id, AttributeName = "", AttributeValue = "", DisplayOrder = 1 };
            
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
             if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được để trống");
             if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải > 0");

            if (!ModelState.IsValid)
            {
                return View("EditAttribute", data);
            }

            if (data.AttributeID == 0)
                ProductDataService.AddAttribute(data);
            else
                ProductDataService.UpdateAttribute(data);

            return RedirectToAction("Edit", new { id = data.ProductID });
        }

        public IActionResult DeleteAttribute(int id, long attributeId = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteAttribute(attributeId);
                return RedirectToAction("Edit", new { id = id });
            }
            var model = ProductDataService.GetAttribute(attributeId);
            if (model == null)
                return RedirectToAction("Edit", new { id = id });
            return View(model);
        }
    }
}
