using GeekShopping.CartAPI.Data.DTO;

namespace GeekShopping.CartAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDTO> GetCouponByCouponCode(string couponCode);
    }
}
