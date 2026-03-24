using System.ComponentModel.DataAnnotations;

namespace Lab01_WebMVC.Models.ViewModels;

public class ProductVM {
    [Required, MaxLength(250)] public string Name { get; set; } = "";
    public string? Description { get; set; }
    [Required] public decimal Price { get; set; }
    public decimal? SalePrice { get; set; }
    public int Stock { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    [Required] public int CategoryId { get; set; }
}

public class BlogPostVM {
    [Required, MaxLength(300)] public string Title { get; set; } = "";
    [MaxLength(600)] public string Summary { get; set; } = "";
    [Required] public string Content { get; set; } = "";
    public bool Publish { get; set; } = false;
    [Required] public int CategoryId { get; set; }
    public int? ProductId { get; set; }
}
