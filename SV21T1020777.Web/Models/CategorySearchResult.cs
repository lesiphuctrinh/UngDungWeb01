using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    public class CategorySearchResult: PaginationSearchResult
    {
        public List<Category> Data { get; set; }
    }
}
