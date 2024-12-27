namespace SV21T1020777.DomainModels
{
    public class Customer
    {
        public int CustomerID {  get; set; }
        public string CustomerName { get; set; } = "";
        public string ContactName { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Address {  get; set; } = string.Empty;
        public string Phone {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsLocked {  get; set; }

        //bổ sung thêm
        public string Photo { get; set; } = string.Empty;
    }
}
