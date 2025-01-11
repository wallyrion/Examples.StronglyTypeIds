using Bogus;
using Examples.StronglyTypedIds.Entities;

namespace Examples.StronglyTypedIds;

public static class Fakers
{
    public static readonly Faker<Owner> Owner = new Faker<Owner>()
        .RuleFor(o => o.Id, f => new OwnerId(f.Random.Guid()))
        .RuleFor(o => o.Name, f => f.Name.FullName());
    
    public static Faker<Pet> Pet (OwnerId ownerId) => new Faker<Pet>()
        .RuleFor(p => p.Id, f => new PetId(f.Random.Guid()))
        .RuleFor(p => p.Name, f => f.Name.FullName())
        .RuleFor(p => p.OwnerId, f => ownerId);
}