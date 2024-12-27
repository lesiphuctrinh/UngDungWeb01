using SV21T1020777.DomainModels;

namespace SV21T1020777.Shop.Models
{
    public class ProductSearchResult: PaginationSearchResult
    {
        public int CategoryID { get; set; } = 0;
        public required List<Product> Data { get; set; }
    }
}
