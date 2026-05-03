using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;
using SMAS.API.Models;
using SMAS.API.DTOs.Customer;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
        {
            var customers = await _context.Customers
                .Include(c => c.Orders)
                .ToListAsync();

            var result = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                City = c.City,
                Province = c.Province,
                CreatedAt = c.CreatedAt,
                TotalOrders = c.Orders.Count
            });

            return Ok(result);
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetById(Guid id)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound("Customer not found ");

            var result = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                City = customer.City,
                Province = customer.Province,
                CreatedAt = customer.CreatedAt,
                TotalOrders = customer.Orders.Count
            };

            return Ok(result);
        }

        // GET: api/customer/city/{city}
        [HttpGet("city/{city}")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> 
            GetByCity(string city)
        {
            var customers = await _context.Customers
                .Include(c => c.Orders)
                .Where(c => c.City != null && 
                    c.City.ToLower() == city.ToLower())
                .ToListAsync();

            var result = customers.Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                City = c.City,
                Province = c.Province,
                CreatedAt = c.CreatedAt,
                TotalOrders = c.Orders.Count
            });

            return Ok(result);
        }

        // POST: api/customer
        [HttpPost]
        public async Task<ActionResult<CustomerDto>> Create(
            CreateCustomerDto dto)
        {
            var exists = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email);

            if (exists)
                return BadRequest("email registered ");

            var customer = new Customer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                City = dto.City,
                Province = dto.Province
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var result = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                City = customer.City,
                Province = customer.Province,
                CreatedAt = customer.CreatedAt,
                TotalOrders = 0
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                result);
        }

        // PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDto>> Update(
            Guid id, UpdateCustomerDto dto)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound("Customer not found");

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;
            customer.City = dto.City;
            customer.Province = dto.Province;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                City = customer.City,
                Province = customer.Province,
                CreatedAt = customer.CreatedAt,
                TotalOrders = customer.Orders.Count
            };

            return Ok(result);
        }

        // DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound("Customer not found ");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok("Customer deleted");
        }
    }
}