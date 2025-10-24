using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;
using System.Reflection;

namespace GeekShopping.Web.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        public const string BasePath = "api/Cart";

        public CartService(HttpClient client)
        {
            _client = client;
        }

        public async Task<CartViewModel> FindCart()
        {
            var response = await _client.GetAsync($"{BasePath}/find-cart");
            return await response.ReadContentAs<CartViewModel>();
        }

        public async Task<CartViewModel> AddItemToCart(CartViewModel cart)
        {
            var response = await _client.PostAsJson($"{BasePath}/add-cart", cart);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else throw new Exception("Something went wrong when calling API." + response.Content.ToString());
        }

        public async Task<CartViewModel> UpdateCart(CartViewModel model)
        {
            var response = await _client.PutAsJson($"{BasePath}/update-cart", model);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartViewModel>();
            else throw new Exception("Something went wrong when calling API.");
        }
        public async Task<bool> RemoveFromCart(long cartId)
        {
            var response = await _client.DeleteAsync($"{BasePath}/remove-cart/{cartId}");

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> ApplyCoupon(CartViewModel model)
        {
            var response = await _client.PostAsJson($"{BasePath}/apply-coupon", model);

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else throw new Exception("Something went wrong when calling API");
        }
        public async Task<bool> RemoveCoupon()
        {
            var response = await _client.DeleteAsync($"{BasePath}/remove-coupon");

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<bool>();
            else throw new Exception("Something went wrong when calling API");
        }

        public async Task<object> Checkout(CartHeaderViewModel cartHeader)
        {
            var response = await _client.PostAsJson($"{BasePath}/checkout", cartHeader);
            var statusCode = response.StatusCode.ToString();

            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<CartHeaderViewModel>();
            else if (statusCode.Equals("PreconditionFailed"))
                return "Coupon Price has changed, please confirm!";
            else throw new Exception("Something went wrong when calling API");
        }

        public async Task<bool> ClearCart(CartHeaderViewModel cartHeader)
        {
            throw new NotImplementedException();
        }
    }
}
