using Discount.API.Requests;
using Discount.Application.Models;
using Discount.Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Discount.API.Enpoints;

public static class CuponEndpoint
{
    public static IEndpointRouteBuilder MapCupon(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("cupons")
            .WithTags("Cupons");

        group
            .MapGet("{id}", async (
                [FromRoute] string id,
                [FromHeader] Guid userId,
                [FromServices] ApplicationDbContext context,
                CancellationToken cancellationToken) =>
            {
                var cupon = await context.GetCuponByIdAsync(id, userId, cancellationToken);

                return cupon is null
                    ? Results.NotFound()
                    : Results.Ok(cupon);
            })
            .WithName("GetUserCupon");
        
        group.MapDelete("{id}", async (
                [FromRoute] string id,
                [FromHeader] Guid userId,
                [FromServices] ApplicationDbContext context,
                CancellationToken cancellationToken) =>
            {
                var cupon = await context.GetCuponByIdAsync(id, userId, cancellationToken);

                if (cupon is null)
                    return Results.NotFound();

                var deleted = await context.Cupons
                    .Where(cupon =>
                        cupon.Id == id &&
                        cupon.UserId == userId)
                    .ExecuteDeleteAsync(cancellationToken);

                return deleted > 0
                    ? Results.NoContent()
                    : Results.NotFound();
            });

        group.MapPost("/", async (
            [FromBody] RegisterCuponRequest request,
            [FromServices] ApplicationDbContext context,
            CancellationToken cancellationToken) =>
        {
            var cupon = await context.GetCuponByIdAsync(request.Key, request.UserId, cancellationToken);
            
            if (cupon is not null)
                return Results.BadRequest(new { message = "The cupon already exists" });

            cupon = new Cupon(request.Key, request.UserId, request.DiscountPercent);

            await context.Cupons.AddAsync(cupon, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Results.CreatedAtRoute("GetUserCupon", new { id = cupon.Id }, cupon);
        });
        
        return builder;
    }
}