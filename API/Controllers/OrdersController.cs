using API.DTOs;
using API.Extensions;
using Core.Entites;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit, UserManager<AppUser> userManager) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
    {
        var user = await userManager.GetUserWithAddressAsync(User);

        if (user == null || string.IsNullOrEmpty(user.Email))
            return Unauthorized("User email not found");

        var userEmail = User.GetEmail();

        var cart = await cartService.GetCartAsync(orderDto.CartId);

        if (cart == null) return BadRequest("Cart not found");

        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order");

        var items = new List<OrderItem>();

        foreach (var item in cart.Items)
        {
            var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);

            if (productItem == null) return BadRequest("Problem with the order");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl
            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quanitity = item.Quantity,
            };
            items.Add(orderItem);
        }

        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);

        if (deliveryMethod == null) return BadRequest("No delivery method selected");

        var order = new Order
        {
            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = orderDto.ShippingAddress,
            SubTotal = items.Sum(x => x.Price * x.Quanitity),
            PaymentSummary = orderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = userEmail,
        };

        unit.Repository<Order>().Add(order);

        if (await unit.Complete())
        {
            return order;
        }

        return BadRequest("Problem creating the order");
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
    {
        var user = await userManager.GetUserWithAddressAsync(User);

        if (user == null || string.IsNullOrEmpty(user.Email))
            return Unauthorized("User email not found");

        var userEmail = user.Email;

        var spec = new OrderSpecification(userEmail);

        var orders = await unit.Repository<Order>().ListAsync(spec);

        var ordersToReturn = orders.Select(o => o.ToDto()).ToList();

        return Ok(ordersToReturn);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var user = await userManager.GetUserWithAddressAsync(User);

        if (user == null || string.IsNullOrEmpty(user.Email))
            return Unauthorized("User email not found");

        var userEmail = user.Email;

        var spec = new OrderSpecification(userEmail, id);

        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);

        if (order == null) return NotFound();

        return order.ToDto();
    }
}
