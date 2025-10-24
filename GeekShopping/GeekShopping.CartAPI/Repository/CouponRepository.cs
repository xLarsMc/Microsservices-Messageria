using AutoMapper;
using GeekShopping.CartAPI.Data.DTO;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;
        public CouponRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<CouponDTO> GetCouponByCouponCode(string couponCode)
        {
            //"api/coupon";
            var response = await _client.GetAsync($"api/coupon/{couponCode}");

            var content = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK) return new CouponDTO();

            return JsonSerializer.Deserialize<CouponDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        }
    }
}
