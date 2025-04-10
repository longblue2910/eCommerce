// src/Services/InventoryService/API/Controllers/InventoryController.cs
using InventoryService.Application.DTOs;
using InventoryService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryApplicationService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryApplicationService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("products/{productId}")]
    [ProducesResponseType(typeof(ProductInventoryStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductInventory(Guid productId)
    {
        var productQuantities = new List<ProductQuantityDto>
        {
            new() { ProductId = productId, Quantity = 1 }
        };

        var results = await _inventoryService.CheckInventoryAsync(productQuantities);

        if (results.Count == 0 || results[0] == null)
            return NotFound();

        return Ok(results[0]);
    }

    [HttpPost("check")]
    [ProducesResponseType(typeof(List<ProductInventoryStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckInventory([FromBody] List<ProductQuantityDto> products)
    {
        var results = await _inventoryService.CheckInventoryAsync(products);
        return Ok(results);
    }

    [HttpPost("reserve/{orderId}")]
    [ProducesResponseType(typeof(List<ProductReservationResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReserveInventory(Guid orderId, [FromBody] List<ProductQuantityDto> products)
    {
        var results = await _inventoryService.ReserveInventoryAsync(orderId, products);
        return Ok(results);
    }

    [HttpPost("release/{orderId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReleaseInventory(Guid orderId)
    {
        var result = await _inventoryService.ReleaseInventoryAsync(orderId);
        return Ok(result);
    }

    [HttpPost("confirm/{orderId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmInventory(Guid orderId)
    {
        var result = await _inventoryService.ConfirmInventoryAsync(orderId);
        return Ok(result);
    }

    [HttpGet("history/{productId}")]
    [ProducesResponseType(typeof(List<InventoryHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInventoryHistory(
        Guid productId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var history = await _inventoryService.GetInventoryHistoryAsync(productId, start, end);
        return Ok(history);
    }
}
