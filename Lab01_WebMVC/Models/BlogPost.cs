namespace Lab01_WebMVC.Models
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int ViewCount { get; set; } = 0;
        public PostStatus Status { get; set; } = PostStatus.Draft;
        public DateTime? PublishedAt { get; set; }
        public int CategoryId { get; set; }
        public int? ProductId { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public Category Category { get; set; } = null!;
        public Product? Product { get; set; }
        public ApplicationUser Author { get; set; } = null!;
        public ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();
    }

    public enum PostStatus { Draft, Published, Archived }
}
