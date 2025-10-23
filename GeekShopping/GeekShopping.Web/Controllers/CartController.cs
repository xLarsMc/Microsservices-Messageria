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
        private readonly ICouponService _couponService;
        public CartController(ILogger<CartController> logger, IProductService productService, ICartService cartService, ICouponService couponService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
            _couponService = couponService;
        }
        public async Task<IActionResult> CartIndex()
        {
            return View(await FindCart());
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartViewModel model)
        {
            var response = await _cartService.ApplyCoupon(model);

            if (response) return RedirectToAction(nameof(CartIndex));

            return View();
        }
        
        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon()
        {
            var response = await _cartService.RemoveCoupon();

            if (response) return RedirectToAction(nameof(CartIndex));

            return View();
        }

        public async Task<IActionResult> Remove(int id)
        {

            var response = await _cartService.RemoveFromCart(id);

            if (response) return RedirectToAction(nameof(CartIndex));

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return View(await FindCart());
        }
        private async Task<CartViewModel> FindCart()
        {
            var response = await _cartService.FindCart();

            if (response?.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(response.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(response.CartHeader.CouponCode);

                    if(coupon?.CouponCode != null)
                    {
                        response.CartHeader.DiscountAmount = coupon.DiscountAmount;
                    }
                }
                foreach (var detail in response.CartDetails)
                {
                    response.CartHeader.PurchaseAmount += (detail.Product.Price * detail.Count);
                }

                response.CartHeader.PurchaseAmount -= response.CartHeader.DiscountAmount;
            }
            return response;
        }
    }
}
