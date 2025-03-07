using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharedKernel.Common;

namespace Domain.Entities;

[BsonIgnoreExtraElements]
public class Supplier(ObjectId id, string name, string contactInfo) : AggregateRoot<ObjectId>(id)
{
    [BsonElement("name")]
    public string Name { get; private set; } = name;

    [BsonElement("contactInfo")]
    public string ContactInfo { get; private set; } = contactInfo;
}