using Lab01_WebMVC.Models;
using Lab01_WebMVC.Helpers;

namespace Lab01_WebMVC.Services;

public class CartService : ICartService {
    private const string KEY = "cart";

    public List<CartItem> GetCart(ISession s)
        => s.Get<List<CartItem>>(KEY) ?? new();

    public void AddItem(ISession s, CartItem newItem)
    {
        var cart = GetCart(s);
        var existing = cart.FirstOrDefault(i=>i.ProductId==newItem.ProductId);
        if (existing is not null) existing.Quantity += newItem.Quantity;
        else cart.Add(newItem);
        s.Set(KEY, cart);
    }

    public void UpdateQty(ISession s, int productId, int quantity)
    {
        var cart = GetCart(s);
        var item = cart.FirstOrDefault(i=>i.ProductId==productId);
        if (item is not null) { item.Quantity=quantity; if(item.Quantity<=0) cart.Remove(item); }
        s.Set(KEY, cart);
    }

    public void RemoveItem(ISession s, int productId)
    { var c=GetCart(s); c.RemoveAll(i=>i.ProductId==productId); s.Set(KEY,c); }

    public void Clear(ISession s) => s.Remove(KEY);
    public int GetCount(ISession s) => GetCart(s).Sum(i=>i.Quantity);
    public decimal GetTotal(ISession s) => GetCart(s).Sum(i=>i.Total);
}
