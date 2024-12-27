using SV21T1020777.DomainModels;

namespace SV21T1020777.Web.Models
{
    public class EmployeeSearchResult: PaginationSearchResult
    {
        public List<Employee> Data { get; set; }
    }
}
