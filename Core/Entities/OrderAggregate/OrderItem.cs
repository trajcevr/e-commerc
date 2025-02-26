using System;
using Core.Entites;

namespace Core.Entities.OrderAggregate;

public class OrderItem : BaseEntity
{
    public ProductItemOrdered ItemOrdered { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quanitity { get; set; }
}
