using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020777.BusinessLayers;
using SV21T1020777.DomainModels;
using SV21T1020777.Web.AppCodes;
using SV21T1020777.Web.Models;

namespace SV21T1020777.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINISTRATOR},{WebUserRoles.EMPLOYEE}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                    SearchValue = ""
                };
            return View(condition);
        }
        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListOfProducts(out rowCount, condition.Page, condition.PageSize,
                                           condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, 
                                           condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var data = new Product()
            {
                ProductID = 0, // id bằng 0 thì bổ sung
            };
            return View("Edit", data);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
        [HttpPost]
        public IActionResult Save(Product data, IFormFile? _Photo)
        {
            try
            {
                //xử lí ảnh
                if (_Photo != null)
                {
                    //lưu ảnh
                    string fileName = $"{DateTime.Now.Ticks} - {_Photo.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images/products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _Photo.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                // Kiểm tra dữ liệu đầu vào không hợp lệ và tạo thông báo lỗi
                ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";

                if (string.IsNullOrWhiteSpace(data.ProductName))
                    ModelState.AddModelError(nameof(data.ProductName), "Tên sản phẩm không để trống");

                if (string.IsNullOrWhiteSpace(data.Unit))
                    ModelState.AddModelError(nameof(data.Unit), "Đơn vị tính không được để trống");

                if (data.CategoryID <= 0)
                    ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng chọn loại hàng");

                if (data.SupplierID <= 0)
                    ModelState.AddModelError(nameof(data.SupplierID), "Vui lòng chọn nhà cung cấp");

                if (data.Price <= 0)
                    ModelState.AddModelError(nameof(data.Price), "Giá sản phẩm phải lớn hơn 0");

                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError(nameof(data.Photo), "Vui lòng cung chọn cho sản phẩm");

                // dựa vào thuộc tính IsValid của ModelState để biết có tồn tại lỗi hay không?
                if (ModelState.IsValid == false)
                {
                    return View("Edit", data);
                }

                if (data.ProductID == 0)
                {
                    // bổ sung
                    int id = ProductDataService.AddProduct(data);
                    if (id <= 0)
                    {
                        return View("Edit", data);
                    }
                    //chuyển đến view eidt của mặt hàng vừa bổ sung
                    return RedirectToAction("Edit", new { id = id });
                }
                else
                {
                    // cập nhật
                    bool result = ProductDataService.UpdateProduct(data);
                    if ((result == false))
                    {
                        return View("Edit", data);
                    }
                }
                return RedirectToAction("Index");
            }catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Edit", data);
            }
        }
        public IActionResult Delete(int id = 0)
        {
            if(Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);
            if(data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    return View(new ProductPhoto { ProductID = id }); 

                case "edit":
                    ViewBag.Title = "Thay đổi ảnh của mặt hàng";
                    var data = ProductDataService.GetPhoto(photoId); 
                    if (data == null) return RedirectToAction("Edit", new { id = id });
                    return View(data); 

                case "delete":
                    ProductDataService.DeletePhoto(photoId); 
                    return RedirectToAction("Edit", new { id = id });

                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile _Photo)
        {
            try
            {

                // Xử lý ảnh
                if (_Photo != null)
                {
                    string fileName = $"{DateTime.Now.Ticks} - {_Photo.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images/products", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        _Photo.CopyTo(stream);
                    }
                    data.Photo = fileName;
                }

                // Kiểm tra dữ liệu đầu vào
                ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh của mặt hàng";
                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError(nameof(data.Photo), "Vui lòng chọn ảnh");

                if (string.IsNullOrWhiteSpace(data.Description))
                    ModelState.AddModelError(nameof(data.Description), "Mô tả không được trống");

                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải lớn hơn không");

                // Kiểm tra DisplayOrder trùng
                if (data.PhotoID == 0 && ProductDataService.CheckPhotoDisplayOrderExists(data.ProductID, data.DisplayOrder))
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã tồn tại. Vui lòng chọn thứ tự khác.");
                }

                if (!ModelState.IsValid)
                {
                    return View("Photo", data);
                }

                if (data.PhotoID == 0)
                {
                    ProductDataService.AddPhoto(data);
                }
                else
                {
                    ProductDataService.UpdatePhoto(data);
                }

                // Quay về view Edit của sản phẩm
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Photo", data);
            }
        }

        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    return View(new ProductAttribute { ProductID = id });

                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính của mặt hàng";
                    var data = ProductDataService.GetAttribute(attributeId);
                    if (data == null) return RedirectToAction("Edit", new { id = id });
                    return View(data);

                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", id);

                default:
                    return RedirectToAction("Edit", new { id = id });
            }
        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính cho mặt hàng" : "Thay đổi thuộc tính của mặt hàng";
                if (string.IsNullOrWhiteSpace(data.AttributeName))
                    ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không để trống");

                if (string.IsNullOrWhiteSpace(data.AttributeValue))
                    ModelState.AddModelError(nameof(data.AttributeValue), "Giá trị thuộc tính không để trống");

                if (data.DisplayOrder <= 0)
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải lớn hơn không");
                // Kiểm tra trùng DisplayOrder
                if (data.AttributeID == 0 && ProductDataService.CheckAttributeDisplayOrderExists(data.ProductID, data.DisplayOrder))
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã tồn tại. Vui lòng chọn thứ tự khác.");
                }

                if (!ModelState.IsValid)
                {
                    return View("Attribute", data);
                }

                if (data.AttributeID == 0)
                {
                    ProductDataService.AddAtribute(data);
                }
                else
                {
                    ProductDataService.UpdateAtribute(data);
                }

                // Quay về view Edit của sản phẩm
                return RedirectToAction("Edit", new { id = data.ProductID });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View("Attribute", data);
            }
        }

    }
}
