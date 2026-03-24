using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity;

namespace Lab01_WebMVC.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider svc)
        {
            var ctx = svc.GetRequiredService<AppDbContext>();
            var rm = svc.GetRequiredService<RoleManager<IdentityRole>>();
            var um = svc.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles
            foreach (var r in new[] { "Admin", "Customer" })
                if (!await rm.RoleExistsAsync(r)) await rm.CreateAsync(new IdentityRole(r));

            // Admin
            if (await um.FindByEmailAsync("admin@shop.vn") is null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@shop.vn",
                    Email = "admin@shop.vn",
                    FullName = "Administrator",
                    EmailConfirmed = true
                };
                await um.CreateAsync(admin, "Admin@123");
                await um.AddToRoleAsync(admin, "Admin");
            }

            if (!await ctx.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Điện thoại", Slug = "dien-thoai", Description = "Các dòng điện thoại mới nhất" },
                    new Category { Name = "Laptop", Slug = "laptop", Description = "Máy tính xách tay cấu hình cao" },
                    new Category { Name = "Phụ kiện", Slug = "phu-kien", Description = "Tai nghe, sạc, cáp..." }
                };

                await ctx.Categories.AddRangeAsync(categories);
                await ctx.SaveChangesAsync();
            }

            if (!await ctx.Products.AnyAsync())
            {
                var cat = await ctx.Categories.FirstAsync();
                var products = new List<Product>
                {
                    new Product { 
                        Name = "iPhone 15 Pro Max", Slug = "iphone-15-pro-max", Price = 29990000, 
                        Stock = 50, CategoryId = cat.Id, ImageUrl = "https://picsum.photos/seed/iphone/400/300", IsFeatured = true
                    },
                    new Product { 
                        Name = "MacBook Air M2", Slug = "macbook-air-m2", Price = 24500000, 
                        Stock = 30, CategoryId = cat.Id, ImageUrl = "https://picsum.photos/seed/macbook/400/300", IsFeatured = true
                    },
                    new Product { 
                        Name = "Sony WH-1000XM5", Slug = "sony-wh-1000xm5", Price = 8490000, 
                        Stock = 100, CategoryId = cat.Id, ImageUrl = "https://picsum.photos/seed/sony/400/300"
                    }
                };

                await ctx.Products.AddRangeAsync(products);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
