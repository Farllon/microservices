using Discount.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Discount.Infra.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Cupon> Cupons { get; set; } = default!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public Task<Cupon?> GetCuponByIdAsync(string cuponId, Guid userId, CancellationToken cancellationToken)
        => Cupons.FirstOrDefaultAsync(cupon => cupon.Id == cuponId && cupon.UserId == userId, cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}