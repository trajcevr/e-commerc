namespace Core.Entities.OrderAggregate;

public class AppCoupon
{
    public required string Name { get; set; }
    public decimal ?AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
    public required string PromotionCode { get; set; }
    public required string CouponId { get; set; }

}
