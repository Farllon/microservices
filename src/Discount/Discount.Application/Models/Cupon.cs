using Discount.Application.Enums;

namespace Discount.Application.Models;

public class Cupon(string id, Guid userId, float discountPercent)
{
    public string Id { get; private set; } = id;

    public Guid UserId { get; private set; } = userId;

    public float DiscountPercent { get; private set; } = discountPercent;
    
    public CuponStatus Status { get; set; } = CuponStatus.ToUse;
}