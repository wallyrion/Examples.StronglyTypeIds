namespace Examples.StronglyTypedIds.Entities;

public class Owner
{
    public OwnerId Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Pet> Pets { get; set; } = [];
}