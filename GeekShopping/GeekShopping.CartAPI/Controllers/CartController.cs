using GeekShopping.CartAPI.Data.ValueObjects;
using GeekShopping.CartAPI.Messages;
using GeekShopping.CartAPI.RabbitMQSender;
using GeekShopping.CartAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private IRabbitMQMessageSender _rabbitMQMessageSender;

        public CartController(ICartRepository cartRepository, IRabbitMQMessageSender rabbitMQMessageSender, ICouponRepository couponRepository)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _rabbitMQMessageSender = rabbitMQMessageSender ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));
            _couponRepository = couponRepository;
        }

        [HttpGet("find-cart/{id}")]
        //[Authorize]
        public async Task<ActionResult<CartVO>> FindById(string id)
        {
            var cart = await _cartRepository.FindCartByUserId(id);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("add-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> AddCart(CartVO vo)
        {
            var cart = await _cartRepository.SaveOrUpdate(vo);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPut("update-cart")]
        [Authorize]
        public async Task<ActionResult<CartVO>> UpdateCart(CartVO vo)
        {
            var cart = await _cartRepository.SaveOrUpdate(vo);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> RemoveCart(int id)
        {
            var status = await _cartRepository.RemoveFromCart(id);
            if (!status) return BadRequest();
            return Ok(status);
        }

        [HttpPost("apply-coupon")]
        [Authorize]
        public async Task<ActionResult<CartVO>> ApplyCoupon(CartVO vo)
        {
            var status = await _cartRepository.ApplyCupon(vo.CartHeader.UserId!, vo.CartHeader.CouponCode!);
            if (!status) return NotFound();
            return Ok(status);
        }

        [HttpDelete("remove-coupon/{userId}")]
        [Authorize]
        public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
        {
            var status = await _cartRepository.RemoveCupon(userId);
            if (!status) return NotFound();
            return Ok(status);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO vo)
        {
            string token = Request.Headers["Authorization"]!;

            if (vo?.UserId == null) return BadRequest();
            var cart = await _cartRepository.FindCartByUserId(vo.UserId!);
            if (cart == null) return NotFound();

            if (!string.IsNullOrEmpty(vo.CouponCode))
            {
                CouponVO coupon = await _couponRepository.GetCouponByCouponCode(vo.CouponCode, token);
                if (vo.DiscountAmount != coupon.DiscountAmount)
                {
                    return StatusCode(412); // 412 status code precondition changed
                }
            }

            vo.CartDetails = cart.CartDetails!;
            vo.DateTime = DateTime.Now;

            // Task RabbitMQ Logigc comes here!!
            await _rabbitMQMessageSender.SendMessage(vo, "checkoutqueue");

            return Ok(vo);
        }
    }
}
