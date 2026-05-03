using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;
using SMAS.API.DTOs.StockAlert;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockAlertController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StockAlertController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/stockalert
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockAlertDto>>> GetAll()
        {
            var alerts = await _context.StockAlerts
                .Include(a => a.Product)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();

            var result = alerts.Select(a => new StockAlertDto
            {
                Id = a.Id,
                ProductName = a.Product?.Name ?? "N/A",
                ProductSKU = a.Product?.SKU ?? "N/A",
                TriggeredAt = a.TriggeredAt,
                CurrentStock = a.CurrentStock,
                ReorderLevel = a.ReorderLevel,
                IsResolved = a.IsResolved,
                ResolvedAt = a.ResolvedAt
            });

            return Ok(result);
        }

        // GET: api/stockalert/unresolved
        [HttpGet("unresolved")]
        public async Task<ActionResult<IEnumerable<StockAlertDto>>> 
            GetUnresolved()
        {
            var alerts = await _context.StockAlerts
                .Include(a => a.Product)
                .Where(a => !a.IsResolved)
                .OrderByDescending(a => a.TriggeredAt)
                .ToListAsync();

            var result = alerts.Select(a => new StockAlertDto
            {
                Id = a.Id,
                ProductName = a.Product?.Name ?? "N/A",
                ProductSKU = a.Product?.SKU ?? "N/A",
                TriggeredAt = a.TriggeredAt,
                CurrentStock = a.CurrentStock,
                ReorderLevel = a.ReorderLevel,
                IsResolved = a.IsResolved,
                ResolvedAt = a.ResolvedAt
            });

            return Ok(result);
        }

        // PUT: api/stockalert/{id}/resolve
        [HttpPut("{id}/resolve")]
        public async Task<ActionResult> Resolve(Guid id)
        {
            var alert = await _context.StockAlerts.FindAsync(id);
            if (alert == null)
                return NotFound("Alert not found!");

            alert.IsResolved = true;
            alert.ResolvedAt = DateTime.UtcNow;
            alert.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok("Alert resolved successfully!");
        }
    }
}