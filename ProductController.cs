using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;
using SMAS.API.Models;
using SMAS.API.DTOs.Product;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                UnitPrice = p.UnitPrice,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel,
                CategoryName = p.Category?.Name ?? "N/A",
                SupplierName = p.Supplier?.CompanyName ?? "N/A",
                CreatedAt = p.CreatedAt,
                IsLowStock = p.StockQuantity <= p.ReorderLevel
            });

            return Ok(result);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found !");

            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                ReorderLevel = product.ReorderLevel,
                CategoryName = product.Category?.Name ?? "N/A",
                SupplierName = product.Supplier?.CompanyName ?? "N/A",
                CreatedAt = product.CreatedAt,
                IsLowStock = product.StockQuantity <= product.ReorderLevel
            };

            return Ok(result);
        }

        // GET: api/product/lowstock
        [HttpGet("lowstock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.StockQuantity <= p.ReorderLevel)
                .ToListAsync();

            var result = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                UnitPrice = p.UnitPrice,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel,
                CategoryName = p.Category?.Name ?? "N/A",
                SupplierName = p.Supplier?.CompanyName ?? "N/A",
                CreatedAt = p.CreatedAt,
                IsLowStock = true
            });

            return Ok(result);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create(
            CreateProductDto dto)
        {
            // Category check
            var category = await _context.Categories
                .FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Category dont  exist !");

            // Supplier check
            var supplier = await _context.Suppliers
                .FindAsync(dto.SupplierId);
            if (supplier == null)
                return BadRequest("Supplier dont exist !");

            var product = new Product
            {
                Name = dto.Name,
                SKU = dto.SKU,
                UnitPrice = dto.UnitPrice,
                StockQuantity = dto.StockQuantity,
                ReorderLevel = dto.ReorderLevel,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Low stock alert check
            if (product.StockQuantity <= product.ReorderLevel)
            {
                var alert = new StockAlert
                {
                    ProductId = product.Id,
                    CurrentStock = product.StockQuantity,
                    ReorderLevel = product.ReorderLevel,
                    IsResolved = false
                };
                _context.StockAlerts.Add(alert);
                await _context.SaveChangesAsync();
            }

            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                ReorderLevel = product.ReorderLevel,
                CategoryName = category.Name,
                SupplierName = supplier.CompanyName,
                CreatedAt = product.CreatedAt,
                IsLowStock = product.StockQuantity <= product.ReorderLevel
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                result);
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(
            Guid id, UpdateProductDto dto)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found !");

            var category = await _context.Categories
                .FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Category dont exist!");

            var supplier = await _context.Suppliers
                .FindAsync(dto.SupplierId);
            if (supplier == null)
                return BadRequest("Supplier don't exist !");

            product.Name = dto.Name;
            product.SKU = dto.SKU;
            product.UnitPrice = dto.UnitPrice;
            product.StockQuantity = dto.StockQuantity;
            product.ReorderLevel = dto.ReorderLevel;
            product.CategoryId = dto.CategoryId;
            product.SupplierId = dto.SupplierId;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var result = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                UnitPrice = product.UnitPrice,
                StockQuantity = product.StockQuantity,
                ReorderLevel = product.ReorderLevel,
                CategoryName = category.Name,
                SupplierName = supplier.CompanyName,
                CreatedAt = product.CreatedAt,
                IsLowStock = product.StockQuantity <= product.ReorderLevel
            };

            return Ok(result);
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound("Product not found!");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Product deleted!");
        }
    }
}