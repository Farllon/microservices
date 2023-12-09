using System.Text;
using System.Text.Json;
using Ordering.Contracts.DTOs;
using Ordering.Contracts.Requests;

namespace Checkout.API.Services.Payment;

public class OrderingService(HttpClient client) : IOrderingService
{
    private readonly HttpClient _client = client;

    public async Task<(OrderDTO?, string? location)> RegisterOrderAsync(RegisterOrderRequest request, CancellationToken cancellationToken)
    {
        const string route = "/orders";

        var bytes = JsonSerializer.SerializeToUtf8Bytes(request);
        
        var requestContent = new StringContent(
            Encoding.UTF8.GetString(bytes), 
            Encoding.UTF8, 
            "application/json");

        var response = await _client.PostAsync(
            route, 
            requestContent, 
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return (null, null);

        var order = await response.Content.ReadFromJsonAsync<OrderDTO>(cancellationToken);

        return (order, response.Headers.Location?.ToString());
    }
}