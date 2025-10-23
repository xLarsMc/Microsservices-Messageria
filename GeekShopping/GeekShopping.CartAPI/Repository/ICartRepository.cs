using GeekShopping.CartAPI.Data.DTO;

namespace GeekShopping.CartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDTO> FindCart();
        Task<CartDTO> SaveOrUpdateCart(CartDTO cart);
        Task<bool> RemoveFromCart(long cartDetailsId);
        Task<bool> ApplyCoupon(string couponCode);
        Task<bool> RemoveCoupon(long cartDetailsId);
        Task<bool> ClearCart();
    }
}
