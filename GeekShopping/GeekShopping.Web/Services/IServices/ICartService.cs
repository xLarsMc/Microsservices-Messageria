using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface ICartService
    {
        Task<CartViewModel> FindCart();
        Task<CartViewModel> AddItemToCart(CartViewModel cart);
        Task<CartViewModel> UpdateCart(CartViewModel cart);
        Task<bool> RemoveFromCart(long cartId);

        //----------------------------------------------------------

        Task<bool> ApplyCoupon();
        Task<bool> RemoveCoupon();
        Task<bool> ClearCart(CartHeaderViewModel cartHeader);
        Task<CartViewModel> Checkout(CartHeaderViewModel cartHeader);
    }
}
