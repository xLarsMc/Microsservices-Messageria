namespace GeekShopping.OrderAPI.Data.DTO
{
    public class CartDetailDTO
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public long ProductId { get; set; }
        public virtual ProductDTO product { get; set; }
        public int Count { get; set; }
    }
}
