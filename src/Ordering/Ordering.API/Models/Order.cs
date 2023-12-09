using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Ordering.Contracts.Enums;

namespace Ordering.API.Models;

public class Order(Guid id, Guid userId, Guid checkoutId, IReadOnlyCollection<OrderItem> items, decimal originalPrice, decimal discount)
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; private set; } = id;

    public Guid CheckoutId { get; private set; } = checkoutId;

    public Guid UserId { get; private set; } = userId;

    public IReadOnlyCollection<OrderItem> Items { get; private set; } = items;

    public decimal OriginalPrice { get; private set; } = originalPrice;

    public decimal Discount { get; private set; } = discount;

    public decimal FinalPrice => OriginalPrice - (OriginalPrice * (Discount / 100));

    public OrderStatus Status { get; set; } = OrderStatus.WaitingPaymentRegister;
}