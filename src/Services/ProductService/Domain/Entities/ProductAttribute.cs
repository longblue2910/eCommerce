using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

/// <summary>
/// Thuộc tính sản phẩm (Màu sắc, Kích thước...).
/// </summary>
public class ProductAttribute(string name, string value)
{
    /// <summary>
    /// Tên thuộc tính.
    /// </summary>
    [BsonElement("name")]
    public string Name { get; private set; } = name;

    /// <summary>
    /// Giá trị thuộc tính.
    /// </summary>
    [BsonElement("value")]
    public string Value { get; private set; } = value;
}
