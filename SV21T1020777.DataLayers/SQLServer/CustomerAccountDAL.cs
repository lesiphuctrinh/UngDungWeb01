using Dapper;
using SV21T1020777.DomainModels;
using System.ComponentModel.Design;

namespace SV21T1020777.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @" select CustomerID as UserId,
		                             Email as UserName,
		                             CustomerName as DisplayName,
		                             Photo,
                                     Phone,
		                             RoleNames
                              From Customers
                              Where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string username, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Customers
			                set Password= @Password
			                where Email= @Email
                           ";
                var parameters = new
                {
                    Password = password,
                    Email = username
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAccountDetails(string userId, string displayName, string email, string phoneNumber, string photoPath)
        {
            bool result = false;
            using(var connection = OpenConnection())
            {
                var sql = @"
                                update Customers
                                set CustomerName = @CustomerName,
	                                Phone = @Phone,
	                                Email = @Email,
                                    Photo = @Photo
                                where CustomerID = @CustomerID
                            ";
                var parameters = new
                {
                    CustomerID = userId,
                    CustomerName = displayName,
                    Email = email,
                    Phone = phoneNumber,
                    Photo = photoPath
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
