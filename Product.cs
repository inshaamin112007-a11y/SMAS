using System;
using System.ComponentModel.DataAnnotations;
using SMAS.API.Models;

namespace SMAS.API.Models
{
    public class Product : Entity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        public int ReorderLevel { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}