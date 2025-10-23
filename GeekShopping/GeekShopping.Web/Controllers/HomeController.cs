using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;

namespace GeekShopping.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
    {
        _logger = logger;
        _productService = productService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.FindAllProducts();
        return View(products);
    }
    
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.FindProductById(id);
        return View(product);
    }

    [HttpPost]
    [ActionName("Details")]
    public async Task<IActionResult> DetailsPost(ProductViewModel model)
    {
        CartViewModel cart = new CartViewModel()
        {
            CartHeader = new CartHeaderViewModel()
            {
                CouponCode = ""
            }
        };


        CartDetailViewModel cartDetail = new CartDetailViewModel()
        {
            Count = model.Count,
            ProductId = model.Id,
            Product = await _productService.FindProductById(model.Id),
            CartHeader = new CartHeaderViewModel()
            {
                CouponCode = ""
            },
        };

        List<CartDetailViewModel> cartDetails = new List<CartDetailViewModel>();

        cartDetails.Add(cartDetail);

        cart.CartDetails = cartDetails;
        var response = await _cartService.AddItemToCart(cart);

        if(response != null)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
