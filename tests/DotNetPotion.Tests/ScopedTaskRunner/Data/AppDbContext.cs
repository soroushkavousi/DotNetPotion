using Microsoft.EntityFrameworkCore;

namespace DotNetPotion.Tests.ScopedTaskRunner.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    private AppDbContext() { }

    public AppDbContext(DbContextOptions options) : base(options) { }
}