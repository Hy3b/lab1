using Lab01_WebMVC.Models;

namespace Lab01_WebMVC.Services;

public interface ICartService {
    List<CartItem> GetCart(ISession session);
    void AddItem(ISession session, CartItem item);
    void UpdateQty(ISession session, int productId, int quantity);
    void RemoveItem(ISession session, int productId);
    void Clear(ISession session);
    int GetCount(ISession session);
    decimal GetTotal(ISession session);
}
