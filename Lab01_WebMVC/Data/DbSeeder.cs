using Lab01_WebMVC.Data;
using Lab01_WebMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab01_WebMVC.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categories = new List<Category>
            {
                new Category { Name = "Điện thoại", Slug = "dien-thoai", Description = "Các dòng điện thoại mới nhất" },
                new Category { Name = "Laptop", Slug = "laptop", Description = "Máy tính xách tay cấu hình cao" },
                new Category { Name = "Phụ kiện", Slug = "phu-kien", Description = "Tai nghe, sạc, cáp..." }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                new Product { 
                    Name = "iPhone 15 Pro Max", 
                    Slug = "iphone-15-pro-max", 
                    Price = 29990000, 
                    Stock = 50, 
                    CategoryId = categories[0].Id,
                    ImageUrl = "https://picsum.photos/seed/iphone/400/300",
                    IsFeatured = true
                },
                new Product { 
                    Name = "MacBook Air M2", 
                    Slug = "macbook-air-m2", 
                    Price = 24500000, 
                    Stock = 30, 
                    CategoryId = categories[1].Id,
                    ImageUrl = "https://picsum.photos/seed/macbook/400/300",
                    IsFeatured = true
                },
                new Product { 
                    Name = "Sony WH-1000XM5", 
                    Slug = "sony-wh-1000xm5", 
                    Price = 8490000, 
                    Stock = 100, 
                    CategoryId = categories[2].Id,
                    ImageUrl = "https://picsum.photos/seed/sony/400/300"
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
