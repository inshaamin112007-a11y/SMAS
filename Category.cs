using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public class Category : Entity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        // One Category has many Products
        public ICollection<Product> Products { get; set; } 
            = new List<Product>();
    }
}