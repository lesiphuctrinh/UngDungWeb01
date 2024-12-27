using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    public class SupplierSearchResult : PaginationSearchResult
    {
        public List<Supplier> Data{ get; set; }
    }
}
