using Microsoft.EntityFrameworkCore;
using SMAS.API.Models;

namespace SMAS.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SaleRecord> SalesRecords { get; set; }
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<ForecastRecord> ForecastRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany(e => e.Orders)
                .HasForeignKey(o => o.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleRecord>()
                .HasOne(sr => sr.Product)
                .WithMany()
                .HasForeignKey(sr => sr.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleRecord>()
                .HasOne(sr => sr.Employee)
                .WithMany(e => e.SaleRecords)
                .HasForeignKey(sr => sr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockAlert>()
                .HasOne(sa => sa.Product)
                .WithMany()
                .HasForeignKey(sa => sa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ForecastRecord>()
                .HasOne(fr => fr.Product)
                .WithMany()
                .HasForeignKey(fr => fr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SaleRecord>()
                .Property(sr => sr.Revenue)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Employee>()
                .Property(e => e.MonthlySalesTarget)
                .HasColumnType("decimal(18,2)");
        }
    }
}