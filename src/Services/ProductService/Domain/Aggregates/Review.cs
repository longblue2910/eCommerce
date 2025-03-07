using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Aggregates;

[BsonIgnoreExtraElements]
public class Review(ObjectId customerId, int rating, string comment, ObjectId productId)
{
    [BsonElement("customerId")]
    public ObjectId CustomerId { get; private set; } = customerId;

    [BsonElement("rating")]
    public int Rating { get; private set; } = rating;

    [BsonElement("productId")]
    public ObjectId ProductId { get; private set; } = productId;

    [BsonElement("comment")]
    public string Comment { get; private set; } = comment;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
}