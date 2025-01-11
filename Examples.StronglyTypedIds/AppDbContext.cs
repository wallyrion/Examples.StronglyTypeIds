using Examples.StronglyTypedIds.Entities;
using Microsoft.EntityFrameworkCore;

namespace Examples.StronglyTypedIds;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Pet> Pets { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>(b =>
        {
            b.Property(o => o.Id).HasConversion(new OwnerId.EfCoreValueConverter());
        });

        modelBuilder.Entity<Pet>(b =>
        {
            b.Property(c => c.OwnerId).HasConversion(new OwnerId.EfCoreValueConverter());
            b.Property(c => c.Id).HasConversion(new PetId.EfCoreValueConverter());
        });
    }
}