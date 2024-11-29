
namespace GeekShopping.Web.Models
{
    public class CartDetailViewModel
    {
        public long Id { get; set; }
        public long CartHeaderId { get; set; }
        public virtual CartHeaderViewModel CartHeader { get; set; }
        public long ProductId { get; set; }
        public virtual ProductViewModel Product { get; set; }
        public int Count { get; set; }
    }
}
