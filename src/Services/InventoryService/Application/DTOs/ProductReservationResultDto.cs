// src/Services/InventoryService/Application/DTOs/ProductReservationResultDto.cs
namespace InventoryService.Application.DTOs;

/// <summary>
/// DTO chứa kết quả giữ hàng cho sản phẩm
/// </summary>
public class ProductReservationResultDto
{
    public Guid ProductId { get; set; }
    public bool Reserved { get; set; }
    public string Message { get; set; }
}
