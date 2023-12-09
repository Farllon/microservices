using Basket.API.Extensions;
using Basket.API.Models;
using Basket.API.Requests;
using Basket.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Basket.API.Endpoints;

public static class BasketEndpoint
{
    public static IEndpointRouteBuilder MapBasket(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("baskets")
            .WithTags("Baskets");

        group
            .MapGet("{userId}", async (
                [FromRoute] Guid userId,
                [FromServices] IDatabase database) =>
            {
                var hash = await database.HashGetAllAsync(userId.ToString());

                Models.Basket basket;

                if (hash.Length == 0)
                {
                    basket = new Models.Basket(userId);

                    hash = basket.ToHashEntries();

                    await database.HashSetAsync(userId.ToString(), hash);
                }
                else
                    basket = hash.ToObject<Models.Basket>();

                return Results.Ok(new BasketDTO
                {
                    UserId = basket.UserId,
                    Items = basket.Items
                        .Select(item => new BasketItemDTO(
                            item.ProductId, 
                            item.Name, 
                            item.Price, 
                            item.Quantity))
                });
            })
            .WithName("BetById")
            .Produces<BasketDTO>();
        
        group
            .MapPost("{userId}/items", async (
                [FromRoute] Guid userId,
                [FromBody] BasketItem item,
                [FromServices] IDatabase database) =>
            {
                var hash = await database.HashGetAllAsync(userId.ToString());

                var basket = hash.Length == 0 
                    ? new Models.Basket(userId) 
                    : hash.ToObject<Models.Basket>();

                var basketItem = basket
                    .Items
                    .FirstOrDefault(bi => bi.ProductId == item.ProductId);
                
                if (basketItem is null)
                    basket.AddItem(item);
                else
                    basketItem.IncreaseQuantity(1);

                hash = basket.ToHashEntries();

                await database.HashSetAsync(userId.ToString(), hash);

                return Results.Ok(basket);
            })
            .WithName("AddItem")
            .Produces<Models.Basket>();

        group.MapDelete("{userId}", async (
            [FromRoute] Guid userId,
            [FromServices] IDatabase database) =>
        {
            var hash = await database.HashGetAllAsync(userId.ToString());

            if (hash.Length == 0)
                return Results.NotFound();

            await database.KeyDeleteAsync(userId.ToString());

            return Results.NoContent();
        });
        
        group
            .MapDelete("{userId}/items/{productId}", async (
                [FromRoute] Guid userId,
                [FromRoute] Guid productId,
                [FromServices] IDatabase database) =>
            {
                var hash = await database.HashGetAllAsync(userId.ToString());

                if (hash.Length == 0)
                    return Results.NotFound();
                
                var basket = hash.ToObject<Models.Basket>();
                
                var basketItem = basket
                    .Items
                    .FirstOrDefault(bi => bi.ProductId == productId);
                
                if (basketItem is null)
                    return Results.NotFound();
                
                basketItem.DecreaseQuantity(basketItem.Quantity);
                
                hash = basket.ToHashEntries();

                await database.HashSetAsync(userId.ToString(), hash);

                return Results.Ok(basket);
            })
            .WithName("DeleteIem")
            .Produces<Models.Basket>()
            .Produces(StatusCodes.Status404NotFound);
        
        group
            .MapPatch("{userId}/items/{productId}/increase-quantity", async (
                [FromRoute] Guid userId,
                [FromRoute] Guid productId,
                [FromBody] IncreaseBasketItemQuantityRequest request,
                [FromServices] IDatabase database) =>
            {
                var hash = await database.HashGetAllAsync(userId.ToString());
                
                if (hash.Length == 0)
                    return Results.NotFound();
                
                var basket = hash.ToObject<Models.Basket>();
                
                var basketItem = basket
                    .Items
                    .FirstOrDefault(bi => bi.ProductId == productId);
                
                if (basketItem is null)
                    return Results.NotFound();
                
                basketItem.IncreaseQuantity(request.IncreasingQuantity);
                
                hash = basket.ToHashEntries();

                await database.HashSetAsync(userId.ToString(), hash);

                return Results.Ok(basket);
            })
            .WithName("IncreaseItemQuantity")
            .Produces<Models.Basket>()
            .Produces(StatusCodes.Status404NotFound);
        
        group
            .MapPatch("{userId}/items/{productId}/decrease-quantity", async (
                [FromRoute] Guid userId,
                [FromRoute] Guid productId,
                [FromBody] DecreaseBasketItemQuantityRequest request,
                [FromServices] IDatabase database) =>
            {
                var hash = await database.HashGetAllAsync(userId.ToString());
                
                if (hash.Length == 0)
                    return Results.NotFound();
                
                var basket = hash.ToObject<Models.Basket>();
                
                var basketItem = basket
                    .Items
                    .FirstOrDefault(bi => bi.ProductId == productId);
                
                if (basketItem is null)
                    return Results.NotFound();
                
                basketItem.DecreaseQuantity(request.DecreasingQuantity);
                
                hash = basket.ToHashEntries();

                await database.HashSetAsync(userId.ToString(), hash);

                return Results.Ok(basket);
            })
            .WithName("DecreaseItemQuantity")
            .Produces<Models.Basket>()
            .Produces(StatusCodes.Status404NotFound);
        
        return builder;
    }
}