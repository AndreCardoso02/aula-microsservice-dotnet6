using GeekShopping.CartAPI.Data.ValueObjects;

namespace GeekShopping.CartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartVO> FindCartByUserId(string userId);
        Task<CartVO> SaveOrUpdate(CartVO cart);
        Task<bool> RemoveFromCart(long cartDetailsId);
        Task<bool> ApplyCupon(string userId, string cuponCode);
        Task<bool> RemoveCupon(string userId);
        Task<bool> ClearCart(string userId);        
    }
}
