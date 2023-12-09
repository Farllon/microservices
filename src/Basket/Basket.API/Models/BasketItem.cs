using System.Text.Json.Serialization;

namespace Basket.API.Models;

public class BasketItem
{
    public Guid ProductId { get; set; }
    
    public string Name { get; set; } = default!;

    public decimal Price { get; set; }

    [JsonInclude]
    public int Quantity { get; private set; } = 1;

    [JsonIgnore]
    public bool InBasket => Quantity > 0;

    public void IncreaseQuantity(int value)
    {
        if (Quantity + value < 0)
            throw new InvalidOperationException($"Is not possible decrease quantity more than {Quantity} times");

        Quantity += value;
    }

    public void DecreaseQuantity(int value)
        => IncreaseQuantity(value * -1);
}