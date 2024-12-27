using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    /// <summary>
    /// Biểu diễn dữ liệu sử dụng cho chức năng hiển thị chi tiết của đơn hàng (Order/Details)
    /// </summary>
    public class OrderDetailModel
    {
        public Order? Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
