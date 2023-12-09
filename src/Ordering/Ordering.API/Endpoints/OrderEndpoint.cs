using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Ordering.API.Models;
using Ordering.Contracts.DTOs;
using Ordering.Contracts.Enums;
using Ordering.Contracts.Requests;

namespace Ordering.API.Endpoints;

public static class OrderEndpoint
{
    public static IEndpointRouteBuilder MapOrder(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("orders")
            .WithTags("Orders");

        group
            .MapGet("{id}", async (
                [FromRoute] Guid id,
                [FromServices] IMongoCollection<Order> orders,
                CancellationToken cancellationToken) =>
            {
                var order = await orders
                    .Find(o => o.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                return order is null
                    ? Results.NotFound()
                    : Results.Ok(new OrderDTO(
                        order.Id,
                        order.UserId,
                        order.Items.Select(item => new OrderItemDTO(
                            item.ProductId,
                            item.ProductName,
                            item.Quantity,
                            item.Price)),
                        order.OriginalPrice,
                        order.Discount,
                        order.FinalPrice,
                        order.Status));
            })
            .WithName("GetById");

        group.MapPost("/", async (
            [FromBody] RegisterOrderRequest request,
            [FromServices] IMongoCollection<Order> orders,
            CancellationToken cancellationToken) =>
        {
            var order = await orders
                .Find(o =>
                    o.CheckoutId == request.CheckoutId &&
                    o.UserId == request.UserId)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (order is not null)
                return Results.Ok(new OrderDTO(
                    order.Id,
                    order.UserId,
                    order.Items.Select(item => new OrderItemDTO(
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.Price)),
                    order.OriginalPrice,
                    order.Discount,
                    order.FinalPrice,
                    order.Status));

            order = new Order(
                Guid.NewGuid(),
                request.UserId,
                request.CheckoutId,
                request.Items
                    .Select(item => new OrderItem(
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.Price))
                    .ToList(),
                request.Price,
                request.Discount);

            await orders.InsertOneAsync(order, cancellationToken: cancellationToken);
            
            return Results.CreatedAtRoute(
                "GetById", 
                new { id = order.Id },
                new OrderDTO(
                    order.Id,
                    order.UserId,
                    order.Items.Select(item => new OrderItemDTO(
                        item.ProductId,
                        item.ProductName,
                        item.Quantity,
                        item.Price)),
                    order.OriginalPrice,
                    order.Discount,
                    order.FinalPrice,
                    order.Status));
        });

        group.MapPatch("{id}/change-payment", async (
            [FromRoute] Guid id,
            [FromServices] IMongoCollection<Order> orders,
            CancellationToken cancellationToken) =>
        {
            var order = await orders
                .Find(o => o.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (order is null)
                return Results.NotFound();

            if (order.Status != OrderStatus.WaitingPaymentRegister)
                return Results.BadRequest(new
                {
                    message = "The order payment mehtod alreadys selected"
                });

            order.Status = OrderStatus.Registered;

            await orders.UpdateOneAsync(
                o => o.Id == id,
                Builders<Order>.Update.Set(
                    o => o.Status,
                    OrderStatus.Registered),
                cancellationToken: cancellationToken);
            
            // TODO: iniciar a ordem

            return Results.Ok(new OrderDTO(
                order.Id,
                order.UserId,
                order.Items.Select(item => new OrderItemDTO(
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.Price)),
                order.OriginalPrice,
                order.Discount,
                order.FinalPrice,
                order.Status));
        });
        
        return builder;
    }
}