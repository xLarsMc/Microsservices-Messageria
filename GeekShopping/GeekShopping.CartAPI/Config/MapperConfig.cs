using AutoMapper;
using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            RegisterMaps();
        }

        private void RegisterMaps()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Cart, CartDTO>().ReverseMap();
            CreateMap<CartDetail, CartDetailDTO>().ReverseMap();
            CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
        }
    }
}
