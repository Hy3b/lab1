using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;

namespace Lab01_WebMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles="Admin")]
public class OrderController : Controller {
    private readonly AppDbContext _ctx;

    public OrderController(AppDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(OrderStatus? status, int page=1)
    {
        const int ps = 20;
        var q = _ctx.Orders.Include(o=>o.User).AsQueryable();
        if (status.HasValue) q = q.Where(o=>o.Status == status);
        
        var total = await q.CountAsync();
        var list = await q.OrderByDescending(o=>o.CreatedAt)
            .Skip((page-1)*ps).Take(ps).AsNoTracking().ToListAsync();
            
        ViewBag.Page = page; 
        ViewBag.TotalPages = (int)Math.Ceiling(total/(double)ps);
        ViewBag.Status = status;
        
        return View(list);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var order = await _ctx.Orders.Include(o=>o.User)
            .Include(o=>o.Items).ThenInclude(i=>i.Product)
            .FirstOrDefaultAsync(o=>o.Id == id);
            
        return order is null ? NotFound() : View(order);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _ctx.Orders.FindAsync(id);
        if (order is null) return Json(new{ok=false, message="Order not found"});
        
        order.Status = status;
        order.UpdatedAt = DateTimeOffset.UtcNow;
        await _ctx.SaveChangesAsync();
        
        return Json(new{ok=true, status=status.ToString()});
    }
}
