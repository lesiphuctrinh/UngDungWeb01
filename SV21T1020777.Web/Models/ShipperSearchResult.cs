using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    public class ShipperSearchResult: PaginationSearchResult
    {
        public List<Shipper> Data { get; set; }
    }
}
