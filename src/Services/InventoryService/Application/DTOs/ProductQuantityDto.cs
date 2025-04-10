// src/Services/InventoryService/Application/DTOs/ProductQuantityDto.cs
namespace InventoryService.Application.DTOs;

/// <summary>
/// DTO chứa thông tin số lượng sản phẩm
/// </summary>
public class ProductQuantityDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
