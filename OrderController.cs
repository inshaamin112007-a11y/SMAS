using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMAS.API.Data;
using SMAS.API.Models;
using SMAS.API.DTOs.Order;

namespace SMAS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var result = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerName = o.Customer?.FullName ?? "N/A",
                EmployeeName = o.Employee?.Name ?? "N/A",
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                DeliveryCity = o.DeliveryCity,
                CourierRef = o.CourierRef,
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "N/A",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.SubTotal
                }).ToList()
            });

            return Ok(result);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found!");

            var result = new OrderDto
            {
                Id = order.Id,
                CustomerName = order.Customer?.FullName ?? "N/A",
                EmployeeName = order.Employee?.Name ?? "N/A",
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                DeliveryCity = order.DeliveryCity,
                CourierRef = order.CourierRef,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "N/A",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.SubTotal
                }).ToList()
            };

            return Ok(result);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                return BadRequest("Customer not found!");

            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
                return BadRequest("Employee not found!");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                DeliveryCity = dto.DeliveryCity,
                CourierRef = dto.CourierRef,
                Status = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow
            };

            decimal total = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product not found!");

                if (product.StockQuantity < item.Quantity)
                    return BadRequest(
                        $"Insufficient stock for {product.Name}. " +
                        $"Available: {product.StockQuantity}");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.UnitPrice
                };

                orderItems.Add(orderItem);
                total += orderItem.UnitPrice * orderItem.Quantity;

                // Deduct stock
                product.StockQuantity -= item.Quantity;

                // Check low stock
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
                }
            }

            order.TotalAmount = total;
            order.OrderItems = orderItems;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),
                new { id = order.Id },
                new { message = "Order created successfully!", orderId = order.Id });
        }

        // PUT: api/order/{id}/status
        [HttpPut("{id}/status")]
        public async Task<ActionResult> UpdateStatus(
            Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found!");

            if (!Enum.TryParse<OrderStatus>(dto.Status, out var status))
                return BadRequest("Invalid status!");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok($"Order status updated to {dto.Status}!");
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Order not found!");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok("Order deleted successfully!");
        }
    }
}