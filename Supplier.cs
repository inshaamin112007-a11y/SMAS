using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public class Supplier : Entity
    {
        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ContactName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        // One Supplier has many Products
        public ICollection<Product> Products { get; set; } 
            = new List<Product>();
    }
}