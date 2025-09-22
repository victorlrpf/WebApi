using BackendExample.Models;
using BackendExample.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendExample.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly ISupabaseService _db;
    private readonly IMicroserviceClient _ms;

    public StockController(ISupabaseService db, IMicroserviceClient ms)
    {
        _db = db;
        _ms = ms;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.GetStockAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _db.GetStockItemAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(StockItem item)
    {
        item.Id = Guid.NewGuid();
        item.UpdatedAt = DateTime.UtcNow;
        var created = await _db.CreateStockItemAsync(item);
        if (created == null) return BadRequest();
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("adjust/{id}")]
    public async Task<IActionResult> Adjust(Guid id, [FromQuery] int delta)
    {
        var ok = await _db.UpdateStockQuantityAsync(id, delta);
        if (!ok) return BadRequest();
        return NoContent();
    }

    // Example of forwarding a request to an inventory microservice (e.g. to reserve items)
    [HttpPost("reserve/{id}")]
    public async Task<IActionResult> Reserve(Guid id, [FromBody] object payload)
    {
        var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
        var resp = await _ms.ForwardToInventoryAsync($"/reserve/{id}", HttpMethod.Post, content);
        var text = await resp.Content.ReadAsStringAsync();
        return StatusCode((int)resp.StatusCode, text);
    }
}