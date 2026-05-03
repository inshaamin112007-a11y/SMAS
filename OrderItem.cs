using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public class OrderItem : Entity
    {
        // Foreign Keys
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        // Calculated Property
        public decimal SubTotal => Quantity * UnitPrice;
    }
}