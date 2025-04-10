// src/Services/InventoryService/Application/DTOs/InventoryHistoryDto.cs
namespace InventoryService.Application.DTOs;

/// <summary>
/// DTO chứa thông tin lịch sử thay đổi tồn kho
/// </summary>
public class InventoryHistoryDto
{
    public Guid ProductId { get; set; }
    public int NewQuantity { get; set; }
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; }
}
