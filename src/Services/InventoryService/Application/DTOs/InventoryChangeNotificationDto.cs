// src/Services/InventoryService/Application/DTOs/InventoryChangeNotificationDto.cs
namespace InventoryService.Application.DTOs;

/// <summary>
/// DTO chứa thông tin thông báo thay đổi tồn kho
/// </summary>
public class InventoryChangeNotificationDto
{
    public Guid ProductId { get; set; }
    public int NewQuantity { get; set; }
    public string EventType { get; set; }
    public DateTime Timestamp { get; set; }
}
