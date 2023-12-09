using Basket.API.Attributes;

namespace Basket.API.Models;

public record Basket
{
    public Guid UserId { get; private set; }

    private List<BasketItem> _items = new();

    public IEnumerable<BasketItem> Items
    {
        get => _items.Where(item => item.InBasket);
        private set => _items = value.ToList();
    }

    [IgnoreOnHash]
    public decimal Total => Items.Sum(item => item.Price * item.Quantity);

    private Basket() { }

    public Basket(Guid userId)
    {
        UserId = userId;
    }
    
    public void AddItem(BasketItem item)
        => _items.Add(item);
}