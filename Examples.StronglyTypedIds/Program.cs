using Bogus;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(s =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    s.UseNpgsql(connectionString);
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapScalarApiReference();

app.MapOpenApi();

app.UseHttpsRedirection();



app.MapGet("/owners", async (AppDbContext dbContext) =>
    {
        return await dbContext.Owners.ToListAsync();
    })
    .WithName("Get all owners");

app.MapPost("/owners", async (AppDbContext dbContext) =>
    {
        var owner = Fakers.Owner.Generate();
        await dbContext.Owners.AddAsync(owner);
        await dbContext.SaveChangesAsync();
        return owner;
    })
    .WithName("Create random owner");


app.MapPost("/owners/{ownerId:guid}/pet", async (AppDbContext dbContext, Guid ownerId) =>
    {
        var pet = Fakers.Pet(ownerId).Generate();
        await dbContext.Pets.AddAsync(pet);
        await dbContext.SaveChangesAsync();
        return pet;
    })
    .WithName("Create random pet for owner");


app.MapGet("/owners/{ownerId:guid}/pets", async (AppDbContext dbContext, Guid ownerId) =>
    {
        return await dbContext.Pets.Where(p => p.OwnerId == ownerId).ToListAsync();
    })
    .WithName("Get Pets by owner id");

app.Run();

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Pet> Pets { get; set; }
}

public class Owner
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Pet> Pets { get; set; } = [];
}

public class Pet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;
}

public class Fakers
{
    public static Faker<Owner> Owner = new Faker<Owner>()
        .RuleFor(o => o.Id, f => f.Random.Guid())
        .RuleFor(o => o.Name, f => f.Name.FullName());
    
    
    public static Faker<Pet> Pet (Guid ownerId) => new Faker<Pet>()
        .RuleFor(p => p.Id, f => f.Random.Guid())
        .RuleFor(p => p.Name, f => f.Name.FullName())
        .RuleFor(p => p.OwnerId, f => ownerId);
}