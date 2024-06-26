﻿using CommonPackage;
using Microsoft.AspNetCore.Mvc;
using InventoryService.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        [Route("CreateItem")]
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
        [Route("GetAllItems")]
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
        [HttpGet]
        [Route("GetItemsByIds")]
        public async Task<ActionResult<List<Item>>> GetItemsByIds(List<int> ids)
        {
            using var activity = _tracer.StartActiveSpan("GetItemsByIds controller");
            try
            {
                var items = await _itemService.GetItemsByIds(ids);
                return items;
            }
            catch (Exception ex)
            {
                Monitoring.Log.Error("Unable to retrieve items basing on ids.", ex);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
