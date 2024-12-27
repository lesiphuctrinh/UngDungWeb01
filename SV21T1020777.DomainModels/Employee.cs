namespace SV21T1020777.DomainModels
{
    public class Employee
    {
        public int EmployeeID {  get; set; }
        public string FullName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string Photo { get; set; } = string.Empty;
        public bool IsWorking { get; set; }
    }
}
