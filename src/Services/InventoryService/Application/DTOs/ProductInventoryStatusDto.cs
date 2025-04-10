// src/Services/InventoryService/Application/DTOs/ProductInventoryStatusDto.cs
namespace InventoryService.Application.DTOs;

/// <summary>
/// DTO chứa thông tin trạng thái tồn kho của sản phẩm
/// </summary>
public class ProductInventoryStatusDto
{
    public Guid ProductId { get; set; }
    public bool InStock { get; set; }
    public int AvailableQuantity { get; set; }
    public string Sku { get; set; }
    public string Name { get; set; }
}
