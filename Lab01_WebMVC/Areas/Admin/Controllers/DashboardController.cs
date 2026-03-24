using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;

namespace Lab01_WebMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles="Admin")]
public class DashboardController : Controller {
    private readonly AppDbContext _ctx;

    public DashboardController(AppDbContext ctx) {
        _ctx = ctx;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProducts = await _ctx.Products.CountAsync();
        ViewBag.TotalOrders   = await _ctx.Orders.CountAsync();
        ViewBag.TotalRevenue  = await _ctx.Orders
            .Where(o=>o.Status == OrderStatus.Delivered)
            .SumAsync(o=>o.TotalAmount);
        ViewBag.TotalPosts    = await _ctx.BlogPosts.CountAsync();
        
        var recentOrders = await _ctx.Orders
            .Include(o=>o.User).Include(o=>o.Items)
            .OrderByDescending(o=>o.CreatedAt).Take(5)
            .AsNoTracking().ToListAsync();
            
        return View(recentOrders);
    }
}
