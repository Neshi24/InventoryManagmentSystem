using CommonPackage;
using Microsoft.AspNetCore.Mvc;
using OrderService.Services;
using OpenTelemetry.Trace;
using Shared;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly Tracer _tracer;

        public OrderController(IOrderService orderService, Tracer tracer)
        {
            _orderService = orderService;
            _tracer = tracer;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateOrder controller");
            try
            {
                await _orderService.CreateOrder(orderDto);
                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to create order.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            using var activity = _tracer.StartActiveSpan("GetOrderById controller");
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                    return NotFound("Order not found.");

                return order;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve order.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAllOrders()
        {
            using var activity = _tracer.StartActiveSpan("GetAllOrders controller");
            try
            {
                var orders = await _orderService.GetAllOrders();
                return orders;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve orders.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            using var activity = _tracer.StartActiveSpan("UpdateOrder controller");
            try
            {
                await _orderService.UpdateOrder(id, orderDto);
                return Ok("Order updated successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to update order.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            using var activity = _tracer.StartActiveSpan("DeleteOrder controller");
            try
            {
                await _orderService.DeleteOrder(id);
                return Ok("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to delete order.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("RebuildDB")]
        public IActionResult RebuildDB()
        {
            using var activity = _tracer.StartActiveSpan("RebuildDB controller");
            try
            {
                _orderService.RebuildDB();
                return Ok("Database rebuilt successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to rebuild database.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
