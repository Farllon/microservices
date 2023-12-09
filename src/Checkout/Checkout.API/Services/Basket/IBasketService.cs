using Basket.Contracts.DTOs;

namespace Checkout.API.Services.Basket;

public interface IBasketService
{
    Task<BasketDTO?> GetBasketByIdAsync(Guid id, CancellationToken cancellationToken);

    Task ClearBasket(Guid id, CancellationToken cancellationToken);
}