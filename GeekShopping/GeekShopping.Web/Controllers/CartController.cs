using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        public CartController(ILogger<CartController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {
            
            return View(await FindCart());
        }
        public async Task<IActionResult> Remove(int id)
        {

            var response = await _cartService.RemoveFromCart(id);

            if (response) return RedirectToAction(nameof(CartIndex));

            return View();
        }

        private async Task<CartViewModel> FindCart()
        {
            var response = await _cartService.FindCart();

            if (response?.CartHeader != null)
            {
                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
                }
            }
            return response;
        }
    }
}
