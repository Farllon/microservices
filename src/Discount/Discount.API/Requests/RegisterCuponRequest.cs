namespace Discount.API.Requests;

public record RegisterCuponRequest(
    string Key,
    Guid UserId,
    float DiscountPercent);