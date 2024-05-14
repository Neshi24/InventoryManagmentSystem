using Microsoft.AspNetCore.Mvc;
using InventoryService.Models;
using InventoryService.Services;


namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemDto itemDto)
        {
            try
            {
                await _itemService.CreateItem(itemDto);
                return Ok("Item created successfully.");
            }
            catch (Exception e)
            {
                return BadRequest($"An error occurred: {e.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            try
            {
                var item = await _itemService.GetItemById(id);
                if (item == null)
                    return NotFound("Item not found.");

                return item;
            }
            catch (Exception e)
            {
                return BadRequest($"An error occurred: {e.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Item>>> GetAllItems()
        {
            try
            {
                var items = await _itemService.GetAllItems();
                return items;
            }
            catch (Exception e)
            {
                return BadRequest($"An error occurred: {e.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemDto itemDto)
        {
            try
            {
                await _itemService.UpdateItem(id, itemDto);
                return Ok("Item updated successfully.");
            }
            catch (Exception e)
            {
                return BadRequest($"An error occurred: {e.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                await _itemService.DeleteItem(id);
                return Ok("Item deleted successfully.");
            }
            catch (Exception e)
            {
                return BadRequest($"An error occurred: {e.Message}");
            }
        }
        [HttpPost]
            [Route("RebuildDB")]
            public void RebuildDB()
            {
                _itemService.RebuildDB();
            }
    }
    
}
