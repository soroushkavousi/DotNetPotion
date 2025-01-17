using System.Reflection;
using DotNetPotion.ScopedTaskRunnerPack;
using DotNetPotion.Tests.ScopedTaskRunnerTests.Data;
using DotNetPotion.Tests.ScopedTaskRunnerTests.ProductApplication.AddProduct;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.Tests.ScopedTaskRunnerTests;

public class ScopedTaskRunnerTests
{
    private const string _dbConnectionString = "Host=localhost;Port=5433;Database=TestDb_ScopedTaskRunner;Username=test;Password=Test";

    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScope _serviceScope;
    private readonly IScopedTaskRunner _scopedTaskRunner;
    private readonly IMediator _mediator;
    private readonly AppDbContext _appDbContext;

    public ScopedTaskRunnerTests()
    {
        ServiceCollection services = new();
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
        services.AddScopedTaskRunner();
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_dbConnectionString));

        _serviceProvider = services.BuildServiceProvider();
        _scopedTaskRunner = _serviceProvider.GetService<IScopedTaskRunner>();

        _serviceScope = _serviceProvider.CreateScope();
        _mediator = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();

        _appDbContext = _serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        _appDbContext.Database.EnsureDeleted();
        _appDbContext.Database.EnsureCreated();
    }

    [Fact]
    public async Task AppDbContextInsideMediatR_WhenUseConcurrency_ShouldThrowConcurrencyException()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();
        string[] productNames = ["P1", "P2"];
        List<Task> tasks = [];
        Exception exception = null;

        // Act
        try
        {
            foreach (string productName in productNames)
            {
                tasks.Add(_mediator.Send(new AddProductCommand { Name = productName }));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        // Assert
        Assert.IsAssignableFrom<InvalidOperationException>(exception);
        Assert.Equal(nameof(ConcurrencyDetector), exception.TargetSite?.DeclaringType?.Name);
    }

    [Fact]
    public async Task AppDbContextInsideMediatR_WhenUseConcurrencyWithScopedTaskRunner_ShouldSuccess()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();
        string[] productNames = ["P1", "P2"];
        List<Task> tasks = [];

        // Act
        foreach (string productName in productNames)
        {
            tasks.Add(_scopedTaskRunner.Run(new AddProductCommand { Name = productName }));
        }

        await Task.WhenAll(tasks);

        // Assert
        string[] assertProductNames = await _appDbContext.Products.Select(x => x.Name).ToArrayAsync();
        Assert.Equal(2, assertProductNames.Length);
        Assert.Equal(productNames.ToHashSet(), assertProductNames.ToHashSet());
    }

    [Fact]
    public async Task AppDbContextInsideMediatR_WhenUseConcurrencyWithFireAndForget_ShouldSuccess()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();
        string[] productNames = ["P1", "P2"];

        // Act
        foreach (string productName in productNames)
        {
            _scopedTaskRunner.FireAndForget(new AddProductCommand { Name = productName });
        }

        // Assert
        await Task.Delay(250);
        string[] assertProductNames = await _appDbContext.Products.Select(x => x.Name).ToArrayAsync();
        Assert.Equal(2, assertProductNames.Length);
        Assert.Equal(productNames.ToHashSet(), assertProductNames.ToHashSet());
    }

    [Fact]
    public async Task AppDbContext_WhenUseConcurrencyWithScopedTaskRunner_ShouldSuccess()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();
        string[] productNames = ["P1", "P2"];

        // Act
        foreach (string productName in productNames)
        {
            _scopedTaskRunner.FireAndForget(async scope =>
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                Product product = new(productName);
                appDbContext.Products.Add(product);
                await appDbContext.SaveChangesAsync();
            });
        }

        // Assert
        await Task.Delay(250);
        string[] assertProductNames = await _appDbContext.Products.Select(x => x.Name).ToArrayAsync();
        Assert.Equal(2, assertProductNames.Length);
        Assert.Equal(productNames.ToHashSet(), assertProductNames.ToHashSet());
    }

    [Fact]
    public async Task AppDbContext_WhenNotWaitForATaskAndScopeDisposed_ShouldReturnError()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();

        // Act
        _ = _mediator.Send(new AddProductCommand { Name = "P1", ManualDelay = TimeSpan.FromSeconds(0.3) });
        _serviceScope.Dispose();

        // Assert
        await Task.Delay(TimeSpan.FromSeconds(0.5)); // ensure the command has enough time to persist product
        IServiceScope assertScope = _serviceProvider.CreateScope();
        AppDbContext appDbContext = assertScope.ServiceProvider.GetRequiredService<AppDbContext>();
        bool productExists = await appDbContext.Products.AnyAsync();
        Assert.False(productExists);
    }
}