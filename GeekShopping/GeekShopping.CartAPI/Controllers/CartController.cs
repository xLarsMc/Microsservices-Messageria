using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private ICartRepository _cartRepository;
    private ICouponRepository _couponRepository;
    private IRabbitMQMessageSender _rabbitMQMessageSender;

    public CartController(ICartRepository repository, ICouponRepository couponRepository, IRabbitMQMessageSender rabbitMQMessageSender)
    {
        _cartRepository = repository;
        _couponRepository = couponRepository;
        _rabbitMQMessageSender = rabbitMQMessageSender;
    }

    [HttpGet("find-cart")]
    public async Task<ActionResult<CartDTO>> FindById()
    {
        var cart = await _cartRepository.FindCart();

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


        var cartResult = await _cartRepository.SaveOrUpdateCart(cart);

        if (cartResult == null) return NotFound();

        return Ok(cart);
    }
    
    [HttpPut("update-cart")]
    public async Task<ActionResult<CartDTO>> UpdateCart([FromBody] CartDTO cart)
    {
        var cartResult = await _cartRepository.SaveOrUpdateCart(cart);

        if (cartResult == null) return NotFound();

        return Ok(cart);
    }
    
    [HttpDelete("remove-cart/{id}")]
    public async Task<ActionResult<CartDTO>> RemoveCart(int id)
    {
        var status = await _cartRepository.RemoveFromCart(id);

        if (!status) return BadRequest();

        return Ok(status);
    }

    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartDTO>> ApplyCoupon([FromBody] CartDTO cart)
    {
        var status = await _cartRepository.ApplyCoupon(cart.CartHeader.CouponCode);

        if (!status) return NotFound();

        return Ok(status);
    }
    
    [HttpDelete("remove-coupon")]
    public async Task<ActionResult<CartDTO>> RemoveCoupon()
    {
        var status = await _cartRepository.RemoveCoupon();

        if (!status) return NotFound();

        return Ok(status);
    }
    
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutHeaderDTO>> Checkout(CheckoutHeaderDTO dto)
    {
        var cart = await _cartRepository.FindCart();

        if (cart == null) return NotFound();

        if (!string.IsNullOrEmpty(dto.CouponCode))
        {
            CouponDTO cooupon = await _couponRepository.GetCouponByCouponCode(dto.CouponCode);

            if (dto.DiscountAmount != cooupon.DiscountAmount) return StatusCode(412);
        }

        dto.CartDetails = cart.CartDetails;
        dto.DateTime = DateTime.Now;

        _rabbitMQMessageSender.SendMessage(dto, "checkoutqueue");

        await _cartRepository.ClearCart();

        return Ok(dto);
    }
}
