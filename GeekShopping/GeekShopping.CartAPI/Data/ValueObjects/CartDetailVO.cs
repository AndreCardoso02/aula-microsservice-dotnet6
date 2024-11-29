using GeekShopping.CartAPI.Model.Base;

namespace GeekShopping.CartAPI.Data.ValueObjects
{
    public class CartDetailVO
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public virtual CartHeaderVO CartHeader { get; set; }
        public long ProductId { get; set; }
        public virtual ProductVO Product { get; set; }
        public int Count { get; set; }
    }
}
