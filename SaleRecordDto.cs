namespace SMAS.API.DTOs.SaleRecord
{
    public class SaleRecordDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSaleRecordDto
    {
        public Guid ProductId { get; set; }
        public Guid EmployeeId { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}