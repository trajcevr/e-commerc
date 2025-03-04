using Core.Entities.OrderAggregate;

namespace Core.Interfaces;

public interface ICouponService
{
  Task<AppCoupon?> GetCouponFromPromoCode(string code);
}
