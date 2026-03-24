namespace Lab01_WebMVC.Models
{
    public class BlogComment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public string AuthorId { get; set; } = string.Empty;
        public BlogPost Post { get; set; } = null!;
        public ApplicationUser Author { get; set; } = null!;
    }
}
