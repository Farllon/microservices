namespace Checkout.API.Requests;

public record CheckoutBasketRequest
{
    public Guid BasketId { get; init; }

    public Guid UserId { get; init; }
    
    public IEnumerable<string> Cupons { get; init; } = Enumerable.Empty<string>();
}
    