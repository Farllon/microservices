namespace Checkout.API.Services.Discount;

public class DiscountService(Discounter.DiscounterClient discounter) : IDiscountService
{
    public async Task<decimal> GetDiscountAsync(GetDiscountRequest request, CancellationToken cancellationToken)
    {
        var reply = await discounter.GetDiscountAsync(request, cancellationToken: cancellationToken);

        return (decimal)reply.Percent;
    }

    public async Task GrabCuponsAsync(GrabCuponsRequest request, CancellationToken cancellationToken)
    {
        _ = await discounter.GrabCuponsAsync(request, cancellationToken: cancellationToken);
    }
}