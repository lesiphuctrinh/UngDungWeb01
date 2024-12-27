using Dapper;
using SV21T1020777.DomainModels;

namespace SV21T1020777.DataLayers.SQLServer
{
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using(var connection = OpenConnection())
            {
                var sql = @"insert into Shippers(ShipperName, Phone)
                            values(@ShipperName, @Phone)
                            select SCOPE_IDENTITY()";
                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using(var connection = OpenConnection())
            {
                var sql = @"select count(*)
                            from Shippers
                            where ShipperName like @searchValue";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using(var connection = OpenConnection())
            {
                var sql = @"delete from Shippers
                            where ShipperId = @ShipperId";
                var parameters = new
                {
                    ShipperId = id,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType:System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select * from Shippers
                            where ShipperId = @ShipperId";
                var parameters = new
                {
                    ShipperId = id,
                };
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using( var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where ShipperId = @ShipperId)
                                select 1
                            else
                                select 0";
                var parameters = new
                {
                    ShipperId = id,
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }
        public List<Shipper> List()
        {
            List<Shipper> data = new List<Shipper>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers";
                data = connection.Query<Shipper>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
           List<Shipper> data = new List<Shipper>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from (
	                            select *, ROW_NUMBER() over(order by ShipperName) as RowNumber
	                            from Shippers
	                            where (ShipperName like @searchValue)
	                            )as t
                            where(@pageSize = 0) 
	                            or(t.RowNumber between (@page-1)*@pageSize+1 and @page*@pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Shipper>(sql: sql, param: parameters, commandType:System.Data.CommandType.Text).ToList();
                connection.Close();
            }

                return data;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using( var connection = OpenConnection())
            {
                var sql = @"update Shippers
                            set ShipperName = @ShipperName,
                                Phone = @Phone
                            where ShipperId = @ShipperId";
                var parameters = new
                {
                    ShipperId = data.ShipperID,
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                };
                result = connection.Execute(sql: sql, param: parameters, commandType:System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
