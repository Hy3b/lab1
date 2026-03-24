using System.Security.Claims;
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
public class BlogController : Controller {
    private readonly AppDbContext _ctx;
    private readonly IWebHostEnvironment _env;

    public BlogController(AppDbContext ctx, IWebHostEnvironment env) {
        _ctx = ctx;
        _env = env;
    }

    public async Task<IActionResult> Index(int page=1)
    {
        const int ps = 15;
        var q = _ctx.BlogPosts.Include(b=>b.Author)
                   .Include(b=>b.Category)
                   .Include(b=>b.Product);
                   
        var total = await q.CountAsync();
        var list  = await q.OrderByDescending(b=>b.CreatedAt)
            .Skip((page-1)*ps).Take(ps).AsNoTracking().ToListAsync();
            
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total/(double)ps);
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _ctx.Categories.ToListAsync();
        ViewBag.Products   = await _ctx.Products.Where(p=>p.IsActive).ToListAsync();
        return View(new BlogPostVM());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BlogPostVM vm, IFormFile? imageFile)
    {
        if (!ModelState.IsValid) {
            ViewBag.Categories = await _ctx.Categories.ToListAsync();
            ViewBag.Products   = await _ctx.Products.Where(p=>p.IsActive).ToListAsync();
            return View(vm);
        }
        
        var post = new BlogPost {
            Title = vm.Title, 
            Slug = SlugHelper.Generate(vm.Title),
            Summary = vm.Summary, 
            Content = vm.Content,
            CategoryId = vm.CategoryId, 
            ProductId = vm.ProductId,
            AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
            Status = vm.Publish ? PostStatus.Published : PostStatus.Draft,
            PublishedAt = vm.Publish ? DateTime.UtcNow : null,
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };
        
        if (imageFile is {Length: > 0})
            post.ImageUrl = await SaveImageAsync(imageFile);
            
        _ctx.BlogPosts.Add(post);
        await _ctx.SaveChangesAsync();
        TempData["Ok"] = "Tạo bài viết thành công!";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SaveImageAsync(IFormFile f)
    {
        var dir = Path.Combine(_env.WebRootPath, "uploads", "blogs");
        Directory.CreateDirectory(dir);
        var name = $"{Guid.NewGuid()}{Path.GetExtension(f.FileName)}";
        await using var s = System.IO.File.Create(Path.Combine(dir, name));
        await f.CopyToAsync(s);
        return $"/uploads/blogs/{name}";
    }
}
