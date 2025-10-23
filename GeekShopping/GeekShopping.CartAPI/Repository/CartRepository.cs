using AutoMapper;
using GeekShopping.CartAPI.Data.DTO;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly MySQLContext _context;
        public IMapper _mapper;

        public CartRepository(MySQLContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCoupon(string couponCode)
        {
            var header = await _context.CartHeaders.FirstOrDefaultAsync();

            if (header != null)
            {
                header.CouponCode = couponCode;
                _context.CartHeaders.Update(header);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveCoupon()
        {
            var header = await _context.CartHeaders.FirstOrDefaultAsync();

            if (header != null)
            {
                header.CouponCode = "";
                _context.CartHeaders.Update(header);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ClearCart()
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync();

            if(cartHeader != null)
            {
                _context.CartDetails
                    .RemoveRange(
                    _context.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));

                _context.CartHeaders.Remove(cartHeader);
                await _context.SaveChangesAsync();
                return true;
            } 
            return false;
        }

        public async Task<CartDTO> FindCart()
        {
            var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync();

            Cart cart = new()
            {
                CartHeader = cartHeader,
            };

            cart.CartDetails = await _context.CartDetails
                .Where(c => c.CartHeaderId == cart.CartHeader.Id)
                .Include(c => c.Product).ToListAsync();
            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            try
            {
                CartDetail cartDetail = await _context.CartDetails
                    .FirstOrDefaultAsync(c => c.Id == cartDetailsId);

                int total = _context.CartDetails.Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();

                _context.CartDetails.Remove(cartDetail);

                if (total == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders
                        .FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            ;
        }

        public async Task<CartDTO> SaveOrUpdateCart(CartDTO cartDTO)
        {
            Cart cart = _mapper.Map<Cart>(cartDTO);

            var product = await _context.Products.FirstOrDefaultAsync(
                p => p.Id == cartDTO.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }

            var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync();

            if(cartHeader == null)
            {
                _context.CartHeaders.Add(cart.CartHeader);
                await _context.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;

                _context.Attach(cart.CartHeader);
                var detail = cart.CartDetails.FirstOrDefault();
                detail.CartHeaderId = cart.CartHeader.Id;
                detail.CartHeader = null;
                _context.CartDetails.Add(detail);
                await _context.SaveChangesAsync();
            } 
            else{
                var cartDetail = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                    p => p.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                    p.CartHeaderId == cartHeader.Id);

                if(cartDetail == null)
                {
                    var header = cartHeader;
                    var detail = cart.CartDetails.FirstOrDefault();

                    if(detail != null)
                    {
                        detail.CartHeaderId = header.Id;
                        detail.CartHeader = null;
                        detail.Product = null;     
                        _context.CartDetails.Add(detail);
                        await _context.SaveChangesAsync();
                    }
                } 
                else
                {
                    cartDetail.Product = null;
                    cartDetail.Count += cart.CartDetails.FirstOrDefault().Count;
                    cartDetail.Id = cartDetail.Id;
                    cartDetail.CartHeaderId = cartDetail.CartHeaderId;
                    _context.CartDetails.Update(cartDetail);
                    await _context.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDTO>(cart);
        }
    }
}
