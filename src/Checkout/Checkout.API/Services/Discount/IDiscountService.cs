namespace Checkout.API.Services.Discount;

public interface IDiscountService
{
    Task<decimal> GetDiscountAsync(GetDiscountRequest request, CancellationToken cancellationToken);

    Task GrabCuponsAsync(GrabCuponsRequest request, CancellationToken cancellationToken);
}