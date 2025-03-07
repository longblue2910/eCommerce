using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[BsonIgnoreExtraElements]
public class ProductVariant(string variantName, decimal price)
{
    [BsonElement("variantName")]
    public string VariantName { get; private set; } = variantName;

    [BsonElement("price")]
    public decimal Price { get; private set; } = price;
}