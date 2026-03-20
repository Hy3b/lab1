using Microsoft.AspNetCore.Identity;

namespace Lab01_WebMVC.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //public ICollection<Order> Orders { get; set; } = new List<Order>();
        //public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();

    }
}
