using Ordering.Contracts.DTOs;
using Ordering.Contracts.Requests;

namespace Checkout.API.Services.Payment;

public interface IOrderingService
{
    Task<(OrderDTO?, string? location)> RegisterOrderAsync(RegisterOrderRequest request, CancellationToken cancellationToken);
}