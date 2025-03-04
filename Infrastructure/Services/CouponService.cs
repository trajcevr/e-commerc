using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class CouponService : ICouponService
{
    public CouponService(IConfiguration config)
    {
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
    }
    public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
{
    var promotionService = new PromotionCodeService();
    var options = new PromotionCodeListOptions();
    var promotionCodes = await promotionService.ListAsync(options);

    Console.WriteLine($"Searching for promotion code: {code}");
    foreach (var promo in promotionCodes)
    {
        Console.WriteLine($"Found promo code: {promo.Code} - Coupon ID: {promo.Coupon.Id}");
    }

    var promotionCode = promotionCodes.FirstOrDefault(p => p.Code.ToLower() == code.ToLower());

    if (promotionCode != null && promotionCode.Coupon != null)
    {
        return new AppCoupon
        {
            Name = promotionCode.Coupon.Name,
            AmountOff = promotionCode.Coupon.AmountOff,
            PercentOff = promotionCode.Coupon.PercentOff,
            CouponId = promotionCode.Coupon.Id,
            PromotionCode = promotionCode.Code
        };
    }

    Console.WriteLine("Promotion code not found.");
    return null;
}

}
