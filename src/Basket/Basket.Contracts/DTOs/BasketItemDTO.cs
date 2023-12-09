namespace Basket.Contracts.DTOs;

public record BasketItemDTO(
    Guid ProductId,
    string Name,
    decimal Price,
    int Quantity);