using Ordering.Contracts.DTOs;

namespace Ordering.Contracts.Requests;

public record RegisterOrderRequest(
    Guid UserId,
    Guid CheckoutId,
    IEnumerable<OrderItemDTO> Items,
    decimal Price, 
    decimal Discount);