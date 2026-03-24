using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Lab01_WebMVC.Models.ViewModels;
using Lab01_WebMVC.Services;

namespace Lab01_WebMVC.Controllers;

[Authorize]
public class OrderController : BaseController {
    private readonly AppDbContext _ctx;
    private readonly UserManager<ApplicationUser> _um;

    public OrderController(AppDbContext ctx, ICartService cart, UserManager<ApplicationUser> um) : base(cart) {
        _ctx = ctx;
        _um = um;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var items = _cartService.GetCart(HttpContext.Session);
        if (!items.Any()) return RedirectToAction("Index","Cart");
        var user = await _um.GetUserAsync(User);
        return View(new CheckoutVM {
            FullName = user?.FullName ?? "", 
            Phone = user?.PhoneNumber ?? "" 
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutVM vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var items = _cartService.GetCart(HttpContext.Session);
        if (!items.Any()) return RedirectToAction("Index","Cart");

        await using var tx = await _ctx.Database.BeginTransactionAsync();
        try
        {
            var userId = _um.GetUserId(User);
            if(string.IsNullOrEmpty(userId)) throw new Exception("User not found");

            var order = new Order {
                Code        = "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                FullName    = vm.FullName, 
                Phone       = vm.Phone, 
                Address     = vm.Address,
                Note        = vm.Note,
                TotalAmount = items.Sum(i=>i.Total),
                UserId      = userId,
                CreatedAt   = DateTimeOffset.UtcNow,
                IsDeleted   = false
            };

            foreach (var item in items)
            {
                var product = await _ctx.Products.FindAsync(item.ProductId);
                if (product is null || product.Stock < item.Quantity)
                    throw new Exception($"{item.ProductName} không đủ hàng");
                
                product.Stock -= item.Quantity;
                order.Items.Add(new OrderItem {
                    ProductId = item.ProductId, 
                    Qty = item.Quantity, 
                    UnitPrice = item.Price
                });
            }

            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();
            await tx.CommitAsync();
            
            _cartService.Clear(HttpContext.Session);
            TempData["OrderCode"] = order.Code;
            return RedirectToAction(nameof(Success));
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    public IActionResult Success()
        => View(model: TempData["OrderCode"]?.ToString());

    public async Task<IActionResult> History()
    {
        var userId = _um.GetUserId(User);
        var orders = await _ctx.Orders
            .Include(o=>o.Items).ThenInclude(i=>i.Product)
            .Where(o=>o.UserId==userId)
            .OrderByDescending(o=>o.CreatedAt)
            .AsNoTracking().ToListAsync();
        return View(orders);
    }
}
