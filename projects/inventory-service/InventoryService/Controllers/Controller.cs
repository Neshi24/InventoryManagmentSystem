using CommonPackage;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Services;
using OpenTelemetry.Trace;
using Shared;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly Tracer _tracer;

        public ItemController(IItemService itemService, Tracer tracer)
        {
            _itemService = itemService;
            _tracer = tracer;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemDto itemDto)
        {
            using var activity = _tracer.StartActiveSpan("CreateItem controller");
            try
            {
                await _itemService.CreateItem(itemDto);
                return Ok("Item created successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to create item.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            using var activity = _tracer.StartActiveSpan("GetItemById controller");
            try
            {
                var item = await _itemService.GetItemById(id);
                if (item == null)
                    return NotFound("Item not found.");

                return item;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve item.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Item>>> GetAllItems()
        {
            using var activity = _tracer.StartActiveSpan("GetAllItems controller");
            try
            {
                var items = await _itemService.GetAllItems();
                return items;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemDto itemDto)
        {
            using var activity = _tracer.StartActiveSpan("UpdateItem controller");
            try
            {
                await _itemService.UpdateItem(id, itemDto);
                return Ok("Item updated successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to update item.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            using var activity = _tracer.StartActiveSpan("DeleteItem controller");
            try
            {
                await _itemService.DeleteItem(id);
                return Ok("Item deleted successfully.");
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to delete item.", ex);
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
                _itemService.RebuildDB();
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
