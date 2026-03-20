using Lab01_WebMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lab01_WebMVC.Data
{
    public class AppDbContext:IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var now = DateTimeOffset.UtcNow;
            foreach (var e in ChangeTracker.Entries<BaseEntity>())
            {
                if (e.State == EntityState.Added) e.Entity.CreatedAt = now;
                if (e.State == EntityState.Modified) e.Entity.UpdatedAt = now;
            }
            return await base.SaveChangesAsync(ct);
        }

    }
}
