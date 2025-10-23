using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> FindAllProducts();
        Task <ProductViewModel> FindProductById(long id);
        Task <ProductViewModel> CreateProduct(ProductViewModel product);
        Task <ProductViewModel> UpdateProduct(ProductViewModel product);
        Task <bool> DeleteProductById(long id);
    }
}
