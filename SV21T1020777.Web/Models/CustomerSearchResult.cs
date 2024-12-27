using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    public class CustomerSearchResult: PaginationSearchResult
    {
        public List<Customer> Data { get; set; }
    }
}
