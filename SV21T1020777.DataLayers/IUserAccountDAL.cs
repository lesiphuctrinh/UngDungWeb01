using SV21T1020777.DomainModels;

namespace SV21T1020777.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Xác thực thông tin
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string username, string password);
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(string username, string password);

        bool UpdateAccountDetails(string userId, string displayName, string email, string phoneNumber, string photoPath);

    }
}
