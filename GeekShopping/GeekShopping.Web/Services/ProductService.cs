using GeekShopping.Web.Models;
using GeekShopping.Web.Services.IServices;
using GeekShopping.Web.Utils;

namespace GeekShopping.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _client;
        public const string BasePath = "api/product";

        public ProductService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ProductModel>> FindAllProducts()
        {
            var response = await _client.GetAsync(BasePath);
            return await response.ReadContentAs<List<ProductModel>>();
        }

        public async Task<ProductModel> FindProductById(long id)
        {
            var response = await _client.GetAsync($"{BasePath}/{id}");
            return await response.ReadContentAs<ProductModel>();
        }

        public async Task<ProductModel> CreateProduct(ProductModel product)
        {
            var response = await _client.PostAsJson($"{BasePath}", product);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<ProductModel>();
            else
                throw new Exception("Something went wrong when calling the API");
        }

        public async Task<ProductModel> UpdateProduct(ProductModel product)
        {
            var response = await _client.PutAsJson($"{BasePath}", product);
            if (response.IsSuccessStatusCode)
                return await response.ReadContentAs<ProductModel>();
            else
                throw new Exception("Something went wrong when calling the API");
        }

        public async Task<bool> DeleteProductById(long id)
        {
            var response = await _client.DeleteAsync($"{BasePath}/{id}");
            if (response.IsSuccessStatusCode)
                return true;
            else
                throw new Exception("Something went wrong when calling the API");
        }
    }
}
