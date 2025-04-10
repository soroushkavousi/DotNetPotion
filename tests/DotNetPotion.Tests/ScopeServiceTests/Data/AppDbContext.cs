﻿using Microsoft.EntityFrameworkCore;

namespace DotNetPotion.Tests.ScopeServiceTests.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; init; }

    private AppDbContext() { }

    public AppDbContext(DbContextOptions options) : base(options) { }
}