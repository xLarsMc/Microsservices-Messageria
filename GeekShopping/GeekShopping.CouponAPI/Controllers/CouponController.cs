using GeekShopping.CouponAPI.Data.DTO;
using GeekShopping.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private ICouponRepository _repository;
        public CouponController(ICouponRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{couponCode}")]
        public async Task<ActionResult<CouponDTO>> GetCouponByCouponCode(string couponCode)
        {
            var getCouponCode = await _repository.GetCouponByCouponCode(couponCode);

            if (getCouponCode == null) return NotFound();

            return Ok(getCouponCode);
        }
    }
}
