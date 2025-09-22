using AutoMapper;

namespace GeekShopping.CartAPI.Config
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
        }

        private void ModelToDTO()
        {
        }
    }
}
