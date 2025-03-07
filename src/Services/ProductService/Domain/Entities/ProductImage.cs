using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

/// <summary>
/// Ảnh sản phẩm.
/// </summary>
public class ProductImage(string url, string altText)
{
    /// <summary>
    /// URL của hình ảnh.
    /// </summary>
    [BsonElement("url")]
    public string Url { get; private set; } = url;

    /// <summary>
    /// Mô tả hình ảnh.
    /// </summary>
    [BsonElement("altText")]
    public string AltText { get; private set; } = altText;
}
