using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;
using SMAS.API.Models;
using SMAS.API.DTOs.SaleRecord;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleRecordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SaleRecordController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/salerecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleRecordDto>>> GetAll()
        {
            var records = await _context.SalesRecords
                .Include(s => s.Product)
                .Include(s => s.Employee)
                .ToListAsync();

            var result = records.Select(s => new SaleRecordDto
            {
                Id = s.Id,
                ProductName = s.Product?.Name ?? "N/A",
                EmployeeName = s.Employee?.Name ?? "N/A",
                SaleDate = s.SaleDate,
                QuantitySold = s.QuantitySold,
                Revenue = s.Revenue,
                CreatedAt = s.CreatedAt
            });

            return Ok(result);
        }

        // GET: api/salerecord/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult> GetByEmployee(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                return NotFound("Employee not found!");

            var records = await _context.SalesRecords
                .Include(s => s.Product)
                .Where(s => s.EmployeeId == employeeId)
                .ToListAsync();

            var totalRevenue = records.Sum(s => s.Revenue);
            var totalUnits = records.Sum(s => s.QuantitySold);
            var targetAchieved = employee.MonthlySalesTarget > 0
                ? (totalRevenue / employee.MonthlySalesTarget) * 100
                : 0;

            return Ok(new
            {
                employeeName = employee.Name,
                monthlyTarget = employee.MonthlySalesTarget,
                totalRevenue,
                totalUnitsSold = totalUnits,
                targetAchievedPercent = Math.Round(targetAchieved, 2),
                records = records.Select(s => new SaleRecordDto
                {
                    Id = s.Id,
                    ProductName = s.Product?.Name ?? "N/A",
                    EmployeeName = employee.Name,
                    SaleDate = s.SaleDate,
                    QuantitySold = s.QuantitySold,
                    Revenue = s.Revenue,
                    CreatedAt = s.CreatedAt
                })
            });
        }

        // POST: api/salerecord
        [HttpPost]
        public async Task<ActionResult<SaleRecordDto>> Create(
            CreateSaleRecordDto dto)
        {
            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return BadRequest("Product not found!");

            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
                return BadRequest("Employee not found!");

            var record = new SaleRecord
            {
                ProductId = dto.ProductId,
                EmployeeId = dto.EmployeeId,
                QuantitySold = dto.QuantitySold,
                Revenue = dto.Revenue,
                SaleDate = DateTime.UtcNow
            };

            _context.SalesRecords.Add(record);
            await _context.SaveChangesAsync();

            var result = new SaleRecordDto
            {
                Id = record.Id,
                ProductName = product.Name,
                EmployeeName = employee.Name,
                SaleDate = record.SaleDate,
                QuantitySold = record.QuantitySold,
                Revenue = record.Revenue,
                CreatedAt = record.CreatedAt
            };

            return CreatedAtAction(nameof(GetAll),
                new { id = record.Id }, result);
        }
    }
}