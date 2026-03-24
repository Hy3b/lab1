using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Lab01_WebMVC.Services;

namespace Lab01_WebMVC.Controllers;

public class BlogController : BaseController {
    private readonly AppDbContext _ctx;

    public BlogController(AppDbContext ctx, ICartService cart) : base(cart) {
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search, int page=1)
    {
        const int ps = 9;
        var query = _ctx.BlogPosts
            .Where(b=>b.Status==PostStatus.Published)
            .AsQueryable();

        if (categoryId.HasValue) query=query.Where(b=>b.CategoryId==categoryId);
        if (!string.IsNullOrWhiteSpace(search))
            query=query.Where(b=>b.Title.Contains(search) || b.Summary.Contains(search));

        var total = await query.CountAsync();
        var posts  = await query
            .Include(b=>b.Category).Include(b=>b.Author)
            .Include(b=>b.Product)
            .OrderByDescending(b=>b.PublishedAt)
            .Skip((page-1)*ps).Take(ps)
            .AsNoTracking().ToListAsync();

        ViewBag.Page       = page;
        ViewBag.TotalPages = (int)Math.Ceiling(total/(double)ps);
        ViewBag.Search     = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = await _ctx.Categories.Where(c=>c.IsActive)
            .AsNoTracking().ToListAsync();
        
        return View(posts);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var post = await _ctx.BlogPosts
            .Include(b=>b.Category).Include(b=>b.Author)
            .Include(b=>b.Product)
            .Include(b=>b.Comments).ThenInclude(c=>c.Author)
            .FirstOrDefaultAsync(b=>b.Slug==slug && b.Status==PostStatus.Published);
            
        if (post is null) return NotFound();
        
        post.ViewCount++;
        await _ctx.SaveChangesAsync();

        var related = await _ctx.BlogPosts
            .Where(b=>b.CategoryId==post.CategoryId && b.Id!=post.Id && b.Status==PostStatus.Published)
            .OrderByDescending(b=>b.PublishedAt)
            .Take(3).AsNoTracking().ToListAsync();
            
        ViewBag.Related = related;
        return View(post);
    }

    [HttpPost, Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return RedirectToAction(nameof(Detail), new { slug = "" });
            
        var post = await _ctx.BlogPosts.FindAsync(postId);
        if (post is null) return NotFound();
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId != null)
        {
            _ctx.BlogComments.Add(new BlogComment {
                PostId   = postId,
                Content  = content.Trim(),
                AuthorId = userId,
                CreatedAt = DateTimeOffset.UtcNow,
                IsDeleted = false
            });
            await _ctx.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Detail), new { slug = post.Slug });
    }
}
