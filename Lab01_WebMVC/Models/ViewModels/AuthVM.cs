using System.ComponentModel.DataAnnotations;

namespace Lab01_WebMVC.Models.ViewModels;

public class LoginVM {
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required] public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class RegisterVM {
    [Required, MaxLength(100)] public string FullName  { get; set; } = "";
    [Required, EmailAddress]   public string Email     { get; set; } = "";
    [Required, MinLength(6)]   public string Password  { get; set; } = "";
    [Compare("Password")]      public string Confirm   { get; set; } = "";
}

public class CheckoutVM {
    [Required] public string FullName { get; set; } = "";
    [Required] public string Phone { get; set; } = "";
    [Required] public string Address { get; set; } = "";
    public string? Note { get; set; }
}

public class PaginationVM {
    public int Page { get; set; }
    public int TotalPages { get; set; }
}
