namespace SV21T1020777.DataLayers
{/// <summary>
/// Định nghĩa chức năng đơn giản
/// </summary>
/// <typeparam name="T"></typeparam>
    public interface ISimpleQueryDAL<T>where T: class
    {
        /// <summary>
        /// Try vấn và lấy toàn bộ dữ liệu
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
