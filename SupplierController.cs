using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;
using SMAS.API.Models;
using SMAS.API.DTOs.Supplier;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SupplierController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/supplier
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.Products)
                .ToListAsync();

            var result = suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                CompanyName = s.CompanyName,
                ContactName = s.ContactName,
                Phone = s.Phone,
                City = s.City,
                Country = s.Country,
                CreatedAt = s.CreatedAt,
                ProductCount = s.Products.Count
            });

            return Ok(result);
        }

        // GET: api/supplier/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetById(Guid id)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
                return NotFound("Supplier not found !");

            var result = new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                City = supplier.City,
                Country = supplier.Country,
                CreatedAt = supplier.CreatedAt,
                ProductCount = supplier.Products.Count
            };

            return Ok(result);
        }

        // POST: api/supplier
        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Create(
            CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                Phone = dto.Phone,
                City = dto.City,
                Country = dto.Country
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            var result = new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                City = supplier.City,
                Country = supplier.Country,
                CreatedAt = supplier.CreatedAt,
                ProductCount = 0
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = supplier.Id },
                result);
        }

        // PUT: api/supplier/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<SupplierDto>> Update(
            Guid id, UpdateSupplierDto dto)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
                return NotFound("Supplier nahi mila!");

            supplier.CompanyName = dto.CompanyName;
            supplier.ContactName = dto.ContactName;
            supplier.Phone = dto.Phone;
            supplier.City = dto.City;
            supplier.Country = dto.Country;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = new SupplierDto
            {
                Id = supplier.Id,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                Phone = supplier.Phone,
                City = supplier.City,
                Country = supplier.Country,
                CreatedAt = supplier.CreatedAt,
                ProductCount = supplier.Products.Count
            };

            return Ok(result);
        }

        // DELETE: api/supplier/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);

            if (supplier == null)
                return NotFound("Supplier not found !");

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return Ok("Supplier deleted!");
        }
    }
}