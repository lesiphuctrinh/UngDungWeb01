using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    public class ShippingModel
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public required OrderDetail OrderDetail { get; set; }
        public required List<Shipper> Shippers { get; set; }
    }
}
