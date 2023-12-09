using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models;

public record Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public required string Name { get; set; } = default!;
    
    public string? Description { get; set; }

    public required decimal Price { get; set; }

    public bool Available { get; set; } = true;
}