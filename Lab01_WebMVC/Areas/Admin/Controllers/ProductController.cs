using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Lab01_WebMVC.Models.ViewModels;
using Lab01_WebMVC.Helpers;

namespace Lab01_WebMVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles="Admin")]
public class ProductController : Controller {
    private readonly AppDbContext _ctx;
    private readonly IWebHostEnvironment _env;

    public ProductController(AppDbContext ctx, IWebHostEnvironment env) {
        _ctx = ctx;
        _env = env;
    }

    public async Task<IActionResult> Index(string? search, int page=1)
    {
        const int ps = 20;
        var q = _ctx.Products.Include(p=>p.Category).AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(p=>p.Name.Contains(search));
            
        var total = await q.CountAsync();
        var list  = await q.OrderByDescending(p=>p.CreatedAt)
            .Skip((page-1)*ps).Take(ps).IgnoreQueryFilters()
            .AsNoTracking().ToListAsync();
            
        ViewBag.Page = page; 
        ViewBag.TotalPages = (int)Math.Ceiling(total/(double)ps);
        ViewBag.Search = search;
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _ctx.Categories.ToListAsync();
        return View(new ProductVM());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductVM vm, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        { 
            ViewBag.Categories = await _ctx.Categories.ToListAsync(); 
            return View(vm); 
        }

        var product = new Product {
            Name = vm.Name, 
            Description = vm.Description,
            Slug = SlugHelper.Generate(vm.Name),
            Price = vm.Price, 
            SalePrice = vm.SalePrice,
            Stock = vm.Stock, 
            CategoryId = vm.CategoryId,
            IsActive = vm.IsActive, 
            IsFeatured = vm.IsFeatured,
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };
        
        if (imageFile is { Length: > 0 })
            product.ImageUrl = await SaveImageAsync(imageFile, "products");

        _ctx.Products.Add(product);
        await _ctx.SaveChangesAsync();
        TempData["Ok"] = "Thêm sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _ctx.Products.FindAsync(id);
        if (p is null) return NotFound();
        ViewBag.Categories = await _ctx.Categories.ToListAsync();
        return View(new ProductVM { 
            Name = p.Name, 
            Description = p.Description,
            Price = p.Price, 
            SalePrice = p.SalePrice, 
            Stock = p.Stock,
            CategoryId = p.CategoryId, 
            IsActive = p.IsActive, 
            IsFeatured = p.IsFeatured 
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductVM vm, IFormFile? imageFile)
    {
        var product = await _ctx.Products.FindAsync(id);
        if (product is null) return NotFound();
        
        if (!ModelState.IsValid)
        { 
            ViewBag.Categories = await _ctx.Categories.ToListAsync(); 
            return View(vm); 
        }

        product.Name = vm.Name; 
        product.Description = vm.Description;
        product.Slug = SlugHelper.Generate(vm.Name);
        product.Price = vm.Price; 
        product.SalePrice = vm.SalePrice;
        product.Stock = vm.Stock; 
        product.CategoryId = vm.CategoryId;
        product.IsActive = vm.IsActive; 
        product.IsFeatured = vm.IsFeatured;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        if (imageFile is { Length: > 0 })
            product.ImageUrl = await SaveImageAsync(imageFile, "products");

        await _ctx.SaveChangesAsync();
        TempData["Ok"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _ctx.Products.FindAsync(id);
        if (p is null) return Json(new{ok=false});
        p.IsDeleted = true;
        await _ctx.SaveChangesAsync();
        return Json(new{ok=true});
    }

    private async Task<string> SaveImageAsync(IFormFile f, string folder)
    {
        var dir = Path.Combine(_env.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(dir);
        var name = $"{Guid.NewGuid()}{Path.GetExtension(f.FileName)}";
        await using var s = System.IO.File.Create(Path.Combine(dir, name));
        await f.CopyToAsync(s);
        return $"/uploads/{folder}/{name}";
    }
}
