using SV21T1020777.DataLayers;
using SV21T1020777.DataLayers.SQLServer;
using SV21T1020777.DomainModels;

namespace SV21T1020777.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ISimpleQueryDAL<OrderStatus> orderStatusDB;

        /// <summary>
        /// Hàm tạo
        /// </summary>
        static CommonDataService()
        {
           string connectionString = Configuration.ConnectionString;

            provinceDB = new ProvinceDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            shipperDB = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
            orderStatusDB = new OrderStatusDAL(connectionString);

        }


        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng tìm được</param>
        /// <param name="page"> Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên khách hàng hoặc tên giao dịch cần tìm</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers( out int rowCount, int page = 1, int pageSize = 0, string searchValue ="")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Bổ sung khách hàng mới. Hàm trả về mã của khách hàng được bổ sung
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }

        /// <summary>
        /// Lấy thông tin khách hàng dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }

        /// <summary>
        /// Cập nhập 1 khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }

        /// <summary>
        /// Xóa một khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if(customerDB.InUsed(id))
            {
                return false;
            }
            return customerDB.Delete(id);
        }

        /// <summary>
        ///Hàm kiểm tra khách hàng hiện đang có đơn hàng không được xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách người giao hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize,searchValue);
        }

        /// <summary>
        /// Bổ sung người giao hàng mới. Hàm trả về mã của người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }

        /// <summary>
        /// Lấy thông tin người giao hàng dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }

        /// <summary>
        /// Cập nhật 1 người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data) { 
            return shipperDB.Update(data);
        }

        /// <summary>
        /// Xóa 1 người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.InUsed(id))
            {
                return false;
            }
            return shipperDB.Delete(id);
        }

        /// <summary>
        /// Hàm kiểm tra người giao hàng hiện đang có ở đơn hàng không được xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhà cung cấp dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier>ListOfSupplier(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount=supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Bổ sung nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }

        /// <summary>
        /// Lấy thông tin nhà cung cấp dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }

        /// <summary>
        /// Cập nhập 1 nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }

        /// <summary>
        /// Xóa một nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.InUsed(id))
            {
                return false;
            }
            return supplierDB.Delete(id);
        }

        /// <summary>
        ///Hàm kiểm tra nhà cung cấp hiện đang còn sản phẩm không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount=(categoryDB.Count(searchValue));
            return categoryDB.List(page, pageSize, searchValue);

        }

        /// <summary>
        /// Bổ sung loại hàng mới. Trả về mã loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        /// <summary>
        /// Lấy thông tin loại hàng dựa vào mã loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }

        /// <summary>
        /// Cập nhật 1 loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        /// <summary>
        /// xóa 1 loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            if(categoryDB.InUsed(id))
            {
                return false;
            }
            return categoryDB.Delete(id);
        }

        /// <summary>
        /// Hàm kiểm tra loại hàng có id ở bảng mặt hàng không được xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhân viên dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee>ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = (employeeDB.Count(searchValue));
            return employeeDB.List(page, pageSize,searchValue) ;
        }

        /// <summary>
        /// Bổ sung nhân viên mới. Hàm trả về mã nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        /// <summary>
        /// Lấy thông tin nhân viên dựa vào mã
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }

        /// <summary>
        /// Cập nhật 1 nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }

        /// <summary>
        /// Xóa 1 nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if(employeeDB.InUsed(id))
            {
                return false;
            }
            return employeeDB.Delete(id);
        }

        /// <summary>
        /// hàm kiểm tra nhân viên có ở bảng đơn hàng không thể xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }

        /// <summary>
        /// Trả về danh sách các tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List();
        }
        public static List<Category> ListOfCategories()
        {
            return categoryDB.List();
        }
        public static List<Supplier> ListOfSuppliers()
        {
            return supplierDB.List();
        }
        public static List<Customer> ListOfCustomers()
        {
            return customerDB.List();
        }
        public static List<OrderStatus> ListOrderStatus()
        {
            return orderStatusDB.List();
        }
        public static List<Shipper> ListOfShippers()
        {
            return shipperDB.List();
        }
    }
}
