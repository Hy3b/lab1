using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lab01_WebMVC.Models;

namespace Lab01_WebMVC.Data.Configurations
{
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> b)
        {
            b.ToTable("BlogPosts");
            b.Property(p => p.Title).IsRequired().HasMaxLength(300);
            b.Property(p => p.Slug).IsRequired().HasMaxLength(320);
            b.Property(p => p.Summary).HasMaxLength(600);
            b.Property(p => p.Status).HasConversion<string>();
            b.HasIndex(p => p.Slug).IsUnique();
            b.HasQueryFilter(p => !p.IsDeleted);
            b.HasOne(p => p.Category).WithMany(c => c.BlogPosts)
                .HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.Product).WithMany(p => p.BlogPosts)
                .HasForeignKey(p => p.ProductId).OnDelete(DeleteBehavior.SetNull);
            b.HasOne(p => p.Author).WithMany(u => u.BlogPosts)
                .HasForeignKey(p => p.AuthorId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
