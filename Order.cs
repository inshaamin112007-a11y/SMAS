using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order : Entity
    {
        // Foreign Keys
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalAmount { get; set; }

        [MaxLength(100)]
        public string? DeliveryCity { get; set; }

        [MaxLength(100)]
        public string? CourierRef { get; set; }

        // One Order has many OrderItems
        public ICollection<OrderItem> OrderItems { get; set; } 
            = new List<OrderItem>();
    }
}