using Examples.StronglyTypedIds.Entities;
using Microsoft.EntityFrameworkCore;

namespace Examples.StronglyTypedIds;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Pet> Pets { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Owner>();
        modelBuilder.Entity<Owner>().Property(o => o.Id).HasConversion(new OwnerId.EfCoreValueConverter());

        modelBuilder.Entity<Pet>(builder =>
        {
            builder.Property(c => c.OwnerId).HasConversion(new OwnerId.EfCoreValueConverter());
            builder.Property(c => c.Id).HasConversion(new PetId.EfCoreValueConverter());
        });
    }
}