using System.ComponentModel.DataAnnotations;
using Catalog.API.Data;
using Catalog.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Endpoints;

public static class ProductEndpoint
{
    public static IEndpointRouteBuilder MapProducts(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup("products")
            .WithTags("Products");

        group
            .MapGet("/", async (
                [FromServices] ApplicationDbContext context,
                [FromQuery, Range(1, ushort.MaxValue)] ushort page = 1,
                [FromQuery, Range(1, 15)] ushort limit = 5,
                CancellationToken cancellationToken = default) =>
            {
                var products = await context.Products
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return products.Count > 0
                    ? Results.Ok(products)
                    : Results.NoContent();
            })
            .WithName("GetAll")
            .Produces<IEnumerable<Product>>()
            .Produces(StatusCodes.Status204NoContent);

        group
            .MapPost("/", async (
                [FromBody] Product product,
                [FromServices] ApplicationDbContext context,
                CancellationToken cancellationToken) =>
            {
                var created = await context.Products.AddAsync(product, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                return Results.CreatedAtRoute(
                    "GetById",
                    new() { { "id", created.Entity.Id } },
                    created.Entity);
            })
            .WithName("Create")
            .Produces<Product>(StatusCodes.Status201Created);

        group
            .MapGet("{id}", async (
                [FromRoute] Guid id,
                [FromServices] ApplicationDbContext context,
                CancellationToken cancellationToken) =>
            {
                var product = await context.Products.FindAsync(
                    keyValues: new object[] { id }, 
                    cancellationToken: cancellationToken);

                return product is null
                    ? Results.NotFound()
                    : Results.Ok(product);
            })
            .Produces<Product>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetById");

        group
            .MapPut("{id}", async (
                [FromRoute] Guid id,
                [FromBody] Product product,
                [FromServices] ApplicationDbContext context,
                CancellationToken cancellationToken) =>
            {
                var found = await context.Products.FindAsync(
                    keyValues: new object[] { id },
                    cancellationToken: cancellationToken);

                if (found is null)
                    return Results.NotFound();

                found.Name = product.Name;
                found.Description = product.Description;
                found.Price = product.Price;
                found.Available = product.Available;

                await context.SaveChangesAsync(cancellationToken);

                return Results.Ok(found);
            })
            .WithName("Update")
            .Produces<Product>()
            .Produces(StatusCodes.Status404NotFound);

        group
            .MapDelete("{id}", async (
                [FromRoute] Guid id,
                [FromServices] ApplicationDbContext context,
                CancellationToken cancellationToken) =>
            {
                var result = await context.Products
                    .Where(p => p.Id == id)
                    .ExecuteDeleteAsync(cancellationToken);

                return result > 0
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("Delete");
        
        return builder;
    }
}