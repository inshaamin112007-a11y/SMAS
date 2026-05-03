using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SMAS.API.Data;
using SMAS.API.Models;
using SMAS.API.DTOs.Employee;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public EmployeeController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/employee/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == dto.Email);

            if (employee == null)
                return Unauthorized("Email or  password wrong");

            bool validPassword = BCrypt.Net.BCrypt
                .Verify(dto.Password, employee.PasswordHash);

            if (!validPassword)
                return Unauthorized("Email or password wrong");

            var token = GenerateToken(employee);

            return Ok(new
            {
                token,
                employee = new EmployeeDto
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Role = employee.Role.ToString(),
                    HireDate = employee.HireDate,
                    MonthlySalesTarget = employee.MonthlySalesTarget,
                    CreatedAt = employee.CreatedAt
                }
            });
        }

        // GET: api/employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _context.Employees.ToListAsync();

            var result = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Role = e.Role.ToString(),
                HireDate = e.HireDate,
                MonthlySalesTarget = e.MonthlySalesTarget,
                CreatedAt = e.CreatedAt
            });

            return Ok(result);
        }

        // GET: api/employee/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound("Employee not found!");

            var result = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role.ToString(),
                HireDate = employee.HireDate,
                MonthlySalesTarget = employee.MonthlySalesTarget,
                CreatedAt = employee.CreatedAt
            };

            return Ok(result);
        }

        // POST: api/employee
        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create(
            CreateEmployeeDto dto)
        {
            // Email already exists check
            var exists = await _context.Employees
                .AnyAsync(e => e.Email == dto.Email);

            if (exists)
                return BadRequest("email already registered !");

            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                MonthlySalesTarget = dto.MonthlySalesTarget,
                HireDate = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var result = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role.ToString(),
                HireDate = employee.HireDate,
                MonthlySalesTarget = employee.MonthlySalesTarget,
                CreatedAt = employee.CreatedAt
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                result);
        }

        // PUT: api/employee/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> Update(
            Guid id, UpdateEmployeeDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound("Employee not found !");

            employee.Name = dto.Name;
            employee.Email = dto.Email;
            employee.Role = dto.Role;
            employee.MonthlySalesTarget = dto.MonthlySalesTarget;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role.ToString(),
                HireDate = employee.HireDate,
                MonthlySalesTarget = employee.MonthlySalesTarget,
                CreatedAt = employee.CreatedAt
            };

            return Ok(result);
        }

        // DELETE: api/employee/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return NotFound("Employee not found!");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok("Employee deleted!");
        }

        // JWT Token Generator
        private string GenerateToken(Employee employee)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,
                    employee.Id.ToString()),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role.ToString()),
                new Claim(ClaimTypes.Name, employee.Name)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(
                    int.Parse(jwtSettings["ExpiryInDays"]!)),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}