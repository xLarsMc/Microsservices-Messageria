using AutoMapper;
using GeekShopping.ProductAPI.Data.DTO;
using GeekShopping.ProductAPI.Model;

namespace GeekShopping.ProductAPI.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            DTOToModel();
            ModelToDTO();
        }

        private void DTOToModel()
        {
            CreateMap<ProductDTO, Product>();
        }

        private void ModelToDTO()
        {
            CreateMap<Product, ProductDTO>();
        }
    }
}
