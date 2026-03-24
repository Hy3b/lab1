using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab01_WebMVC.Models;

namespace Lab01_WebMVC.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> b)
        {
            b.ToTable("Products");
            b.Property(p => p.Name).IsRequired().HasMaxLength(250);
            b.Property(p => p.Slug).IsRequired().HasMaxLength(270);
            b.Property(p => p.Price).HasColumnType("decimal(18,2)");
            b.Property(p => p.SalePrice).HasColumnType("decimal(18,2)");
            b.HasIndex(p => p.Slug).IsUnique();
            b.HasIndex(p => new { p.CategoryId, p.IsActive });
            b.HasQueryFilter(p => !p.IsDeleted);
            b.HasOne(p => p.Category).WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
