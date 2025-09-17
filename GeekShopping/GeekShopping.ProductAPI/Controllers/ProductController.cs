using GeekShopping.ProductAPI.Data.DTO;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IProductRepository _repository;
        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> FindAll()
        {
            var products = await _repository.FindAll();
            if (products == null) return NotFound();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindById(long id)
        {
            var product = await _repository.FindById(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO productDto)
        {
            if (productDto == null) return BadRequest();
            var product = await _repository.Create(productDto);
            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductDTO productDto)
        {
            if (productDto == null) return BadRequest();
            var product = await _repository.Update(productDto);
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var status = await _repository.Delete(id);
            if (!status) return BadRequest();
            return Ok();
        }
    }
}
