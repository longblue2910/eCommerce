using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SharedKernel.Common;

namespace Domain.Aggregates;

[BsonIgnoreExtraElements]
public class Brand(ObjectId id, string name) : AggregateRoot<ObjectId>(id)
{
    [BsonElement("name")]
    public string Name { get; private set; } = name;
}