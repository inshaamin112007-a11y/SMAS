using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public class Customer : Entity
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Province { get; set; }

        // One Customer has many Orders
        public ICollection<Order> Orders { get; set; } 
            = new List<Order>();
    }
}