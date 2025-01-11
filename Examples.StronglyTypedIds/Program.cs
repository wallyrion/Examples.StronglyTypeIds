using Examples.StronglyTypedIds;
using Examples.StronglyTypedIds.Entities;
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


app.MapPost("/owners/{ownerId}/pet", async (AppDbContext dbContext, OwnerId ownerId) =>
    {
        var pet = Fakers.Pet(ownerId).Generate();
        await dbContext.Pets.AddAsync(pet);
        await dbContext.SaveChangesAsync();
        return pet;
    })
    .WithName("Create random pet for owner");


app.MapGet("/owners/{ownerId}/pets", async (AppDbContext dbContext, OwnerId ownerId) =>
    {
        return await dbContext.Pets.Where(p => p.OwnerId == ownerId).ToListAsync();
    })
    .WithName("Get Pets by owner id");

app.Run();