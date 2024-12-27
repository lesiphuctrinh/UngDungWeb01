using Dapper;
using SV21T1020777.DomainModels;

namespace SV21T1020777.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {

        }
        public UserAccount? Authorize(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select	EmployeeID as UserId,
		                            Email as UserName,
		                            FullName as DisplayName,
		                            Photo,
                                    Phone,
		                            RoleNames
                            from	Employees
                            where	Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }
        public bool ChangePassword(string username, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Employees
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

        /// <summary>
        /// Không cần định nghĩa
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="displayName"></param>
        /// <param name="email"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="birthday"></param>
        /// <param name="photoPath"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool UpdateAccountDetails(string userId, string displayName, string email, string phoneNumber, string photoPath)
        {
            throw new NotImplementedException();
        }
    }
}
