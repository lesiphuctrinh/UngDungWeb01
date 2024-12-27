using SV21T1020777.DataLayers;
using SV21T1020777.DataLayers.SQLServer;
using SV21T1020777.DomainModels;

namespace SV21T1020777.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
          
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách sản phẩm dưới dạng không phân trang
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        //public static List<Product> ListOfProducts(string searchValue = "")
        //{
        //    return productDB.List(); // xem lại sau
        //}

        // <summary>
        /// Tìm kiếm và lấy danh sách sản phẩm dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết tổng số dòng tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên sản phẩm cần tìm</param>
        /// <param name="categoryID">ID danh mục cần lọc</param>
        /// <param name="supplierID">ID nhà cung cấp cần lọc</param>
        /// <param name="minPrice">Giá tối thiểu</param>
        /// <param name="maxPrice">Giá tối đa</param>
        /// <returns>Danh sách sản phẩm phù hợp với điều kiện lọc và phân trang</returns>
        public static List<Product> ListOfProducts( out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice);
        }

        /// <summary>
        /// Lấy thông tin 1 mặt hàng theo mã mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }

        /// <summary>
        /// Kiểm tra DisplayOrder ở photo có bị trùng hay không?
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="displayOrder"></param>
        /// <returns></returns>
        public static bool CheckPhotoDisplayOrderExists(int productId, int displayOrder)
        {
            return productDB.CheckPhotoDisplayOrderExists(productId, displayOrder);
        }
        /// <summary>
        /// Bổ sung 1 mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }

        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            if(productDB.InUsed(productID))
            {
                return false;
            }
            return productDB.Delete(productID);
        }

        /// <summary>
        /// hàm kiểm tra mặt hàng hiện đang có ở bảng chi tiết đơn hàng không thể xóa
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }

        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng, sắp xếp theo thứ tự của DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            //TODO: chưa chắc chắn, cần xem lại sau này.
            return productDB.ListPhotos(productID).ToList();
        }

        /// <summary>
        /// Lấy thông tin ảnh dựa vào id
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }

        /// <summary>
        /// Bổ sung ảnh
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto photo)
        {
            return productDB.AddPhoto(photo);
        }

        /// <summary>
        /// Cập nhật ảnh
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto photo)
        {
            return productDB.UpdatePhoto(photo);
        }

        /// <summary>
        /// Xóa ảnh
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }

        /// <summary>
        /// Lấy danh sách thuộc tính của mặt hàng, sắp xếp theo thứ tự của DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID).ToList();
        }

        /// <summary>
        /// Lấy thông tin thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }

        /// <summary>
        /// Kiểm tra DisplayOrder ở thuộc tính có bị trùng hay không?
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="displayOrder"></param>
        /// <returns></returns>
        public static bool CheckAttributeDisplayOrderExists(int productId, int displayOrder)
        {
            // Gọi phương thức CheckAttributeDisplayOrderExists từ ProductDAL
            return productDB.CheckAttributeDisplayOrderExists(productId, displayOrder);
        }

        /// <summary>
        /// Bổ sung thuộc tính
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static long AddAtribute(ProductAttribute attribute)
        {
            return productDB.AddAttribute(attribute);
        }

        /// <summary>
        /// Cập nhật thuộc tính
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static bool UpdateAtribute(ProductAttribute attribute)
        {
            return productDB.UpdateAttribute(attribute);
        }

        /// <summary>
        /// Xóa thuộc tính
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
