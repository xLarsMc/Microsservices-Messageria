namespace GeekShopping.CartAPI.Data.DTO
{
    public class CartDetailDTO
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public CartHeaderDTO CartHeader { get; set; }
        public long ProductId { get; set; }
        public ProductDTO product { get; set; }
        public int Count { get; set; }
    }
}
