using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ordering.API.Models;

public class OrderItem(Guid productId, string productName, int quantity, decimal price)
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ProductId { get; private set; } = productId;

    public string ProductName { get; private set; } = productName;

    public int Quantity { get; private set; } = quantity;

    public decimal Price { get; private set; } = price;
}