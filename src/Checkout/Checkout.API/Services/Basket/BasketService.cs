using Basket.Contracts.DTOs;

namespace Checkout.API.Services.Basket;

public class BasketService(HttpClient client) : IBasketService
{
    public async Task<BasketDTO?> GetBasketByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string route = "/baskets/{0}";

        var response = await client.GetAsync(
            string.Format(route, id), 
            cancellationToken);

        return response.IsSuccessStatusCode 
            ? await response.Content.ReadFromJsonAsync<BasketDTO>(cancellationToken)
            : null;
    }

    public async Task ClearBasket(Guid id, CancellationToken cancellationToken)
    {
        const string route = "/baskets/{0}";
        
        _ = await client.DeleteAsync(
            string.Format(route, id), 
            cancellationToken);
    }
}