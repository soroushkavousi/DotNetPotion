namespace DotNetPotion.Tests.ScopedTaskRunner.Data;

public class Product
{
    private Product() { }

    public Product(string name)
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }
}