namespace Ordering.Contracts.DTOs;

public record OrderItemDTO(
    Guid ProductId, 
    string ProductName, 
    int Quantity,
    decimal Price);