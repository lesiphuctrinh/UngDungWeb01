using SV21T1020777.DataLayers;
using SV21T1020777.DomainModels;

namespace SV21T1020777.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;

        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            if (userTypes == UserTypes.Employee)
            {
                return employeeAccountDB.Authorize(username, password);
            }
            else
            {
                return customerAccountDB.Authorize(username, password);
            }
        }
        public static bool ChangePassword(UserTypes userType, string username, string password)
        {
            if (userType == UserTypes.Employee)
            {
                return employeeAccountDB.ChangePassword(username, password);
            }
            else
            {
                return customerAccountDB.ChangePassword(username, password);
            }
        }
        public static bool UpdateAccountDetails(UserTypes userType, string userId, string displayName, string email, string phoneNumber, string photoPath)
        {
            if(userType == UserTypes.Customer)
            {
                return customerAccountDB.UpdateAccountDetails(userId, displayName, email, phoneNumber, photoPath);
            }
            else
            {
                return employeeAccountDB.UpdateAccountDetails(userId, displayName, email, phoneNumber, photoPath);
            }
        }
    }

    public enum UserTypes
    {
        Employee,
        Customer,
    }
}
