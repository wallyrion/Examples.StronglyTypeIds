namespace Examples.StronglyTypedIds.Entities;

public class Pet
{
    public PetId Id { get; set; }
    public string Name { get; set; } = null!;
    public OwnerId OwnerId { get; set; }
    public Owner Owner { get; set; } = null!;
}