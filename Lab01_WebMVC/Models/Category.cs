namespace Lab01_WebMVC.Models
{
    public class Category:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    }
}
