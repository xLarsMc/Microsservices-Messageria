using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private ICartRepository _repository;

    public CartController(ICartRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("find-cart")]
    public async Task<ActionResult<CartDTO>> FindById()
    {
        var cart = await _repository.FindCart();

        if (cart == null) return NotFound();

        return Ok(cart);
    }
    
    [HttpPost("add-cart")]
    public async Task<ActionResult<CartDTO>> AddCart([FromBody] CartDTO cart)
    {

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Any())
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToArray() })
                .ToArray();
            return BadRequest(new ValidationProblemDetails(ModelState));
        }


        var cartResult = await _repository.SaveOrUpdateCart(cart);

        if (cartResult == null) return NotFound();

        return Ok(cart);
    }
    
    [HttpPut("update-cart")]
    public async Task<ActionResult<CartDTO>> UpdateCart([FromBody] CartDTO cart)
    {
        var cartResult = await _repository.SaveOrUpdateCart(cart);

        if (cartResult == null) return NotFound();

        return Ok(cart);
    }
    
    [HttpDelete("remove-cart/{id}")]
    public async Task<ActionResult<CartDTO>> RemoveCart(int id)
    {
        var status = await _repository.RemoveFromCart(id);

        if (!status) return BadRequest();

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartDTO>> ApplyCoupon([FromBody] CartDTO cart)
    {
        var status = await _repository.ApplyCoupon(cart.CartHeader.CouponCode);

        if (!status) return NotFound();

        return Ok(status);
    }
    
    [HttpDelete("remove-coupon")]
    public async Task<ActionResult<CartDTO>> RemoveCoupon()
    {
        var status = await _repository.RemoveCoupon();

        if (!status) return NotFound();

        return Ok(status);
    }
}
