// src/Services/InventoryService/Application/DTOs/InventoryDto.cs
using System.Text.Json.Serialization;

namespace InventoryService.Application.DTOs;

/// <summary>
/// DTO chứa thông tin tồn kho cho cache
/// </summary>
public class InventoryDto
{
    public Guid ProductId { get; set; }
    public int TotalQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string Sku { get; set; }
    public string ProductName { get; set; }
}
