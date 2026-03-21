using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Lab01_WebMVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab01_WebMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private const string CART_KEY = "CartSession";

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity
                });
            }

            HttpContext.Session.Set(CART_KEY, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null) cart.Remove(item);
            
            HttpContext.Session.Set(CART_KEY, cart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0) return RemoveFromCart(productId);

            var cart = HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null) item.Quantity = quantity;

            HttpContext.Session.Set(CART_KEY, cart);
            return RedirectToAction(nameof(Index));
        }
    }
}
