using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public enum EmployeeRole
    {
        Admin,
        Manager,
        Salesman
    }

    public class Employee : Entity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public EmployeeRole Role { get; set; } = EmployeeRole.Salesman;

        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public decimal MonthlySalesTarget { get; set; }

        // One Employee has many Orders and SaleRecords
        public ICollection<Order> Orders { get; set; } 
            = new List<Order>();

        public ICollection<SaleRecord> SaleRecords { get; set; } 
            = new List<SaleRecord>();
    }
}