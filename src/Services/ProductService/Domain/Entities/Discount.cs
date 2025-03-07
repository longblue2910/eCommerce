using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[BsonIgnoreExtraElements]
public class Discount
{
    [BsonElement("percentage")]
    public double Percentage { get; private set; } // VD: 10% giảm giá

    [BsonElement("startDate")]
    public DateTime StartDate { get; private set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; private set; }

    public Discount(double percentage, DateTime startDate, DateTime endDate)
    {
        Percentage = percentage;
        StartDate = startDate;
        EndDate = endDate;
    }
}
