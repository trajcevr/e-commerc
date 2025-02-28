using System;

namespace Core.Entities.OrderAggregate;

public enum OrderStatus
{
    Pending,
    PaymentReceived,
    PaymetFailed,
    PaymentMismatch
}
