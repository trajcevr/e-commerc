using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService cartService;
        private readonly IUnitOfWork unit;

        public PaymentService(IConfiguration config, ICartService cartService, IUnitOfWork unit)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
            this.cartService = cartService;
            this.unit = unit;
        }
        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {

            var cart = await cartService.GetCartAsync(cartId)
                ?? throw new Exception("Cart unavailable");

            var shippingPrice = await GetShippingPriceAsync(cart) ?? 0;
            await ValidateCartItemsAsync(cart);
            
            var subtotal = CalculateSubtotal(cart);
            if (cart.Coupon != null)
            {
                subtotal = await ApplyDiscountAsync(cart.Coupon, subtotal);
            }
            
            var total = subtotal + shippingPrice;
            await CreateOrUpdatePaymentIntentAsync(cart, total);
            
            await cartService.SetCartAsync(cart);
            return cart;
        }

        public async Task<string> RefundPayment(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refundService = new RefundService();
            var result = await refundService.CreateAsync(refundOptions);

            return result.Status;
        }

        private async Task CreateOrUpdatePaymentIntentAsync(ShoppingCart cart, long amount)
        {
            var service = new PaymentIntentService();

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]
                };
                var intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions { Amount = amount };
                await service.UpdateAsync(cart.PaymentIntentId, options);
            }
        }

        private async Task<long> ApplyDiscountAsync(AppCoupon appCoupon, long amount)
        {
            var couponService = new Stripe.CouponService();
            var coupon = await couponService.GetAsync(appCoupon.CouponId);

            if (coupon?.PercentOff.HasValue == true)
            {
                var discount = amount * (coupon.PercentOff.Value / 100);
                amount -= (long)discount;
            }

            return amount;
        }

        private long CalculateSubtotal(ShoppingCart cart)
        {
            return (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100));
        }

        private async Task ValidateCartItemsAsync(ShoppingCart cart)
        {
            foreach (var item in cart.Items)
            {
                var product = await unit.Repository<Core.Entites.Product>().GetByIdAsync(item.ProductId)
                    ?? throw new Exception($"Product not found: {item.ProductId}");
                
                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }
        }

        private async Task<long?> GetShippingPriceAsync(ShoppingCart cart)
        {
            if (!cart.DeliveryMethodId.HasValue)
                return null;

            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value)
                ?? throw new Exception("Invalid delivery method");

            return (long)(deliveryMethod.Price * 100);
        }

        
    }
}
