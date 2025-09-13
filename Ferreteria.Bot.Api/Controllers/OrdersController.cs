using Ferreteria.Bot.Api.Data;
using Ferreteria.Bot.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Ferreteria.Bot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public OrdersController(AppDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            // calcular total simple
            order.TotalPrice = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            order.Timestamp = DateTime.UtcNow;
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id));
    }

}
