using Discount.Application.Enums;
using Discount.Infra.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscounterService : Discounter.DiscounterBase
{
    private readonly ApplicationDbContext _context;

    public DiscounterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public override async Task<GetDiscountReply> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var percent = await _context.Cupons
            .AsNoTracking()
            .Where(cupon =>
                request.Cupons.Any(id => id == cupon.Id) &&
                cupon.UserId == Guid.Parse(request.UserId) &&
                cupon.Status == CuponStatus.ToUse)
            .SumAsync(cupon => cupon.DiscountPercent, context.CancellationToken);

        return new GetDiscountReply
        {
            Percent = percent
        };
    }

    public override async Task<GrabCuponsReply> GrabCupons(GrabCuponsRequest request, ServerCallContext context)
    {
        _ = await _context.Cupons
            .Where(cupon =>
                request.Cupons.Any(id => id == cupon.Id) &&
                cupon.UserId == Guid.Parse(request.UserId) &&
                cupon.Status == CuponStatus.ToUse)
            .ExecuteUpdateAsync(exp => exp.SetProperty(c => c.Status, CuponStatus.InUse), context.CancellationToken);

        return new GrabCuponsReply();
    }
}