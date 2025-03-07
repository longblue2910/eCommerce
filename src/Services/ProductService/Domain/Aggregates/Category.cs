using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharedKernel.Common;

namespace Domain.Aggregates;

/// <summary>
/// Danh mục sản phẩm.
/// </summary>
[BsonIgnoreExtraElements]
public class Category(ObjectId id, string name, ObjectId? parentId = null) : AggregateRoot<ObjectId>(id)
{
    /// <summary>
    /// Tên danh mục.
    /// </summary>
    [BsonElement("name")]
    public string Name { get; private set; } = name;

    /// <summary>
    /// Danh mục cha (nếu có).
    /// </summary>
    [BsonElement("parentId")]
    public ObjectId? ParentId { get; private set; } = parentId;
}
