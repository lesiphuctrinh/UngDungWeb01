using Dapper;
using SV21T1020777.DomainModels;

namespace SV21T1020777.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }
        
        public int Add(Product data)
        {
            int id = 0;
            using(var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                           else
                               begin
                                    insert into Products(ProductName, ProductDescription, CategoryID, SupplierID, Unit, Price, Photo, IsSelling)
	                                values(@ProductName, @ProductDescription, @CategoryID, @SupplierID, @Unit, @Price, @Photo, @IsSelling)
                                    select @@IDENTITY
                               end";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }
        public bool CheckAttributeDisplayOrderExists(int productId, int displayOrder)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*)
                        FROM ProductAttributes
                        WHERE ProductID = @ProductID AND DisplayOrder = @DisplayOrder";

                var parameters = new
                {
                    ProductID = productId,
                    DisplayOrder = displayOrder
                };

                int count = connection.ExecuteScalar<int>(sql: sql, param: parameters);
                return count > 0;
            }
        }

            public long AddAttribute(ProductAttribute data)
        {
            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                    values(@ProductID, @AttributeName, @AttributeValue, @DisplayOrder)
                    select @@IDENTITY";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }
        public bool CheckPhotoDisplayOrderExists(int productId, int displayOrder)
        {
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*)
                            FROM ProductPhotos
                            WHERE ProductID = @ProductID AND DisplayOrder = @DisplayOrder";
                var parameters = new
                {
                    ProductID = productId,
                    DisplayOrder = displayOrder
                };
                return connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            }
        }
        public long AddPhoto(ProductPhoto data)
        {
            if (CheckPhotoDisplayOrderExists(data.ProductID, data.DisplayOrder))
            {
                // Trả về -1 nếu DisplayOrder đã tồn tại
                return -1;
            }

            long id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into ProductPhotos(ProductID, Photo,Description, DisplayOrder, IsHidden)
		                            values(@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden)
                            select @@IDENTITY";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                id = connection.ExecuteScalar<long>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT COUNT(*)
                            FROM Products
                            WHERE (@searchValue = '' OR ProductName LIKE @searchValue OR ProductDescription LIKE @searchValue)
                              AND (@categoryID = 0 OR CategoryID = @categoryID)
                              AND (@supplierID = 0 OR SupplierID = @supplierID)
                              AND (Price >= @minPrice)
                              AND (@maxPrice = 0 OR Price <= @maxPrice)
                        ";

                var parameters = new
                {
                    searchValue = searchValue,
                    categoryID = categoryID,
                    supplierID = supplierID,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };

                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using(var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where ProductID = @ProductID;
                            delete from ProductAttributes where ProductID = @ProductID;
                            delete from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeleteAttribute(long attributeId)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes 
                            where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeId,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoId)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos 
                            where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoId,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select * from Products
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID,
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes
                            where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID,
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos
                            where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID,
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
	                            select 1
                            else 
	                            select 0;";
                var parameters = new
                {
                    ProductID = productID,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }
        
        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT *
                            FROM (
                                SELECT *,
                                ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                                FROM Products
                                WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                    AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                    AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                    AND (Price >= @MinPrice)
                                    AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                                ) AS t
                            WHERE (@PageSize = 0)
                            OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
                var parameters = new
                {
                    Page = page,
                    PageSize = pageSize,
                    SearchValue = searchValue,
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                data = connection.Query<Product>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            var data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"select AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder
                            from ProductAttributes
                            where ProductID = @ProductID
                            order by DisplayOrder ASC
                        ";

                var parameters = new 
                { 
                    ProductID = productID 
                };

                data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            var data = new List<ProductPhoto>();

            using (var connection = OpenConnection())
            {
                var sql = @"select PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden
                            from ProductPhotos
                            where ProductID = @ProductID
                            order by DisplayOrder ASC
                        ";
                var parameters = new 
                {
                    ProductID = productID 
                };
                data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Product data)
        {
           bool result = false;
           using(var connection = OpenConnection())
            {
                var sql = @"update Products
	                            set ProductName = @ProductName,
		                            ProductDescription = @ProductDescription,
                                    CategoryID = @CategoryID,
                                    SupplierID = @SupplierID,
		                            Unit = @Unit,
		                            Price = @Price,
		                            Photo = @Photo,
		                            IsSelling = @IsSelling
	                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
           }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductAttributes
	                            set AttributeName = @AttributeName,
		                            AttributeValue = @AttributeValue,
		                            DisplayOrder = @DisplayOrder
	                            where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = data.AttributeID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update ProductPhotos
	                            set Photo = @Photo,
		                            Description = @Description,
		                            DisplayOrder = @DisplayOrder,
		                            IsHidden= @IsHidden
	                            where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = data.PhotoID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
