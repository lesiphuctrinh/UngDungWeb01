namespace SV21T1020777.Shop.Models
{
    public class ProductSearchInput: PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
    }
}
