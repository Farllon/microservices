using Ordering.Contracts.Enums;

namespace Ordering.Contracts.DTOs;

public record OrderDTO(
    Guid Id, 
    Guid UserId, 
    IEnumerable<OrderItemDTO> Items, 
    decimal OriginalPrice, 
    decimal Discount,
    decimal FinalPrice,
    OrderStatus Status);