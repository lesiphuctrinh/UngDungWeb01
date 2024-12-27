using Dapper;
using SV21T1020777.DomainModels;

namespace SV21T1020777.DataLayers.SQLServer
{
    public class OrderStatusDAL : BaseDAL, ISimpleQueryDAL<OrderStatus>
    {
        public OrderStatusDAL(string connectionString) : base(connectionString)
        {
        }

        public List<OrderStatus> List()
        {
            List<OrderStatus> data = new List<OrderStatus>();
            using (var connection = OpenConnection())
            {
                var sql = @"select *from OrderStatus";
                data = connection.Query<OrderStatus>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        
    }
}
