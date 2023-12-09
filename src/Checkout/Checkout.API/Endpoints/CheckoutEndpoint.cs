using Checkout.API.Requests;
using Checkout.API.Services.Basket;
using Checkout.API.Services.Discount;
using Checkout.API.Services.Payment;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Mvc;
using Ordering.Contracts.DTOs;
using Ordering.Contracts.Requests;

namespace Checkout.API.Endpoints;

public static class CheckoutEndpoint
{
    public static IEndpointRouteBuilder MapCheckout(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("checkouts")
            .WithTags("Checkouts");

        group.MapPost("/", async (
            [FromBody] CheckoutBasketRequest request,
            [FromServices] IBasketService basketService,
            [FromServices] IDiscountService discountService,
            [FromServices] IOrderingService orderingService,
            CancellationToken cancellationToken) =>
        {
            var basket = await basketService.GetBasketByIdAsync(
                request.BasketId,
                cancellationToken);
            
            if (basket is null)
                return Results.NotFound();

            decimal discountPercent = 0;

            bool hasCupons = false;

            if (request.Cupons.Any())
            {
                hasCupons = true;
                
                discountPercent = await discountService.GetDiscountAsync(
                    new GetDiscountRequest
                    {
                        Cupons = { request.Cupons },
                        UserId = request.UserId.ToString()
                    }, 
                    cancellationToken);
            }

            var (order, location) = await orderingService.RegisterOrderAsync(
                new RegisterOrderRequest(
                    request.UserId,
                    Guid.NewGuid(),
                    basket.Items.Select(item => new OrderItemDTO(
                        item.ProductId,
                        item.Name,
                        item.Quantity,
                        item.Price)),
                    basket.Total,
                    discountPercent),
                cancellationToken);

            if (order is null)
                return Results.BadRequest(new
                {
                    message = "The order was not created"
                });
            
            await basketService.ClearBasket(
                request.BasketId, 
                cancellationToken);
            
            if (hasCupons)
                await discountService.GrabCuponsAsync(
                    new GrabCuponsRequest
                    {
                        Cupons = { request.Cupons },
                        UserId = request.UserId.ToString()
                    },
                    cancellationToken);

            return Results.Created(location, order);
        });
        
        return builder;
    }
}