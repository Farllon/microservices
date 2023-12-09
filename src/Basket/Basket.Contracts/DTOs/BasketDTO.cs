using System.Text.Json.Serialization;

namespace Basket.Contracts.DTOs;

public record BasketDTO
{
    public required Guid UserId { get; init; }
    
    public required IEnumerable<BasketItemDTO> Items { get; init; }
    
    [JsonIgnore]
    public decimal Total => Items.Sum(item => item.Price);
}