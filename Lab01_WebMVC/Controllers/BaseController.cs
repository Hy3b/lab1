using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Lab01_WebMVC.Services;

namespace Lab01_WebMVC.Controllers;

public abstract class BaseController : Controller {
    protected readonly ICartService _cartService;

    public BaseController(ICartService cartService) {
        _cartService = cartService;
    }

    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        ViewBag.CartCount = _cartService.GetCount(HttpContext.Session);
        base.OnActionExecuting(ctx);
    }
}
