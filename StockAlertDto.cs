namespace SMAS.API.DTOs.StockAlert
{
    public class StockAlertDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public DateTime TriggeredAt { get; set; }
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}