using Microsoft.Data.SqlClient;

namespace SV21T1020777.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp cơ sở (lớp cha) của các lớp cài đặt các phép xử lý dữ liệu trên SQL Server
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL SQL server
        /// </summary>
        protected string connectionString = "";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Tạo và mở kết nối đến CSDL (SQL server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
