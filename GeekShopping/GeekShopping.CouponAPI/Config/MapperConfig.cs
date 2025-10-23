using AutoMapper;
using GeekShopping.CouponAPI.Data.DTO;
using GeekShopping.CouponAPI.Model;

namespace GeekShopping.CouponAPI.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            RegisterMaps();
        }

        private void RegisterMaps()
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
