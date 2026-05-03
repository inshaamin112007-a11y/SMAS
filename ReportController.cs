using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/report/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult> GetDashboard()
        {
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();
            var totalEmployees = await _context.Employees.CountAsync();
            var totalRevenue = await _context.SalesRecords
                .SumAsync(s => s.Revenue);
            var lowStockCount = await _context.Products
                .CountAsync(p => p.StockQuantity <= p.ReorderLevel);
            var unresolvedAlerts = await _context.StockAlerts
                .CountAsync(a => !a.IsResolved);

            return Ok(new
            {
                totalProducts,
                totalOrders,
                totalCustomers,
                totalEmployees,
                totalRevenue,
                lowStockCount,
                unresolvedAlerts
            });
        }

        // GET: api/report/sales-by-city
        [HttpGet("sales-by-city")]
        public async Task<ActionResult> GetSalesByCity()
        {
            var result = await _context.Orders
                .Where(o => o.DeliveryCity != null)
                .GroupBy(o => o.DeliveryCity)
                .Select(g => new
                {
                    city = g.Key,
                    totalOrders = g.Count(),
                    totalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(x => x.totalRevenue)
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/report/top-products
        [HttpGet("top-products")]
        public async Task<ActionResult> GetTopProducts()
        {
            var result = await _context.SalesRecords
                .Include(s => s.Product)
                .GroupBy(s => new { s.ProductId, s.Product!.Name })
                .Select(g => new
                {
                    productName = g.Key.Name,
                    totalUnitsSold = g.Sum(s => s.QuantitySold),
                    totalRevenue = g.Sum(s => s.Revenue)
                })
                .OrderByDescending(x => x.totalRevenue)
                .Take(10)
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/report/employee-performance
        [HttpGet("employee-performance")]
        public async Task<ActionResult> GetEmployeePerformance()
        {
            var employees = await _context.Employees.ToListAsync();
            var result = new List<object>();

            foreach (var emp in employees)
            {
                var sales = await _context.SalesRecords
                    .Where(s => s.EmployeeId == emp.Id)
                    .ToListAsync();

                var totalRevenue = sales.Sum(s => s.Revenue);
                var totalUnits = sales.Sum(s => s.QuantitySold);
                var targetPercent = emp.MonthlySalesTarget > 0
                    ? Math.Round(
                        (totalRevenue / emp.MonthlySalesTarget) * 100, 2)
                    : 0;

                result.Add(new
                {
                    employeeName = emp.Name,
                    role = emp.Role.ToString(),
                    monthlyTarget = emp.MonthlySalesTarget,
                    totalRevenue,
                    totalUnitsSold = totalUnits,
                    targetAchievedPercent = targetPercent
                });
            }

            return Ok(result
                .OrderByDescending(x =>
                    ((dynamic)x).targetAchievedPercent));
        }
    }
}