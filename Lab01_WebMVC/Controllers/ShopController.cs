using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Lab01_WebMVC.Services;
using Lab01_WebMVC.Helpers;

namespace Lab01_WebMVC.Controllers;

public class ShopController : BaseController {
    private readonly AppDbContext _ctx;
    private readonly IMemoryCache _cache;

    public ShopController(AppDbContext ctx, IMemoryCache cache, ICartService cart) : base(cart) {
        _ctx = ctx;
        _cache = cache;
    }

    public async Task<IActionResult> Index(
        int? categoryId, string? search, decimal? minPrice, decimal? maxPrice,
        string sort="newest", int page=1)
    {
        const int ps = 12;
        var query = _ctx.Products.Where(p=>p.IsActive).AsQueryable();

        if (categoryId.HasValue) query = query.Where(p=>p.CategoryId==categoryId);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p=>p.Name.Contains(search)|| (p.Description != null && p.Description.Contains(search)));
        
        if (minPrice.HasValue) query = query.Where(p=>p.Price>=minPrice);
        if (maxPrice.HasValue) query = query.Where(p=>p.Price<=maxPrice);

        query = sort switch {
            "price_asc"  => query.OrderBy(p=>p.Price),
            "price_desc" => query.OrderByDescending(p=>p.Price),
            _            => query.OrderByDescending(p=>p.CreatedAt),
        };

        var total = await query.CountAsync();
        var items = await query.Include(p=>p.Category)
            .Skip((page-1)*ps).Take(ps).AsNoTracking().ToListAsync();

        if (!_cache.TryGetValue("cats", out List<Category>? cats))
        {
            cats = await _ctx.Categories.Where(c=>c.IsActive)
                .OrderBy(c=>c.SortOrder).AsNoTracking().ToListAsync();
            _cache.Set("cats", cats, TimeSpan.FromMinutes(30));
        }

        ViewBag.Categories  = cats;
        ViewBag.CategoryId  = categoryId;
        ViewBag.Search      = search;
        ViewBag.Sort        = sort;
        ViewBag.Page        = page;
        ViewBag.TotalPages  = (int)Math.Ceiling(total/(double)ps);
        return View(items);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var product = await _ctx.Products
            .Include(p=>p.Category)
            .Include(p=>p.BlogPosts.Where(b=>b.Status == PostStatus.Published))
            .FirstOrDefaultAsync(p=>p.Slug==slug && p.IsActive);
        
        if (product is null) return NotFound();

        var viewed = HttpContext.Session.Get<List<int>>("Viewed") ?? new List<int>();
        if (!viewed.Contains(product.Id)) viewed.Insert(0,product.Id);
        if (viewed.Count>10) viewed.RemoveAt(10);
        HttpContext.Session.Set("Viewed", viewed);

        var related = await _ctx.Products
            .Where(p=>p.CategoryId==product.CategoryId && p.Id!=product.Id && p.IsActive)
            .Take(4).AsNoTracking().ToListAsync();
            
        ViewBag.Related = related;
        return View(product);
    }
}
