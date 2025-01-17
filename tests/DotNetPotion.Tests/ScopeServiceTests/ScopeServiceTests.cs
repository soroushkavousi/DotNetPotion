using System.Reflection;
using DotNetPotion.ScopeServicePack;
using DotNetPotion.Tests.ScopeServiceTests.Data;
using DotNetPotion.Tests.ScopeServiceTests.ProductApplication.AddProduct;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.Tests.ScopeServiceTests;

public class ScopeServiceTests
{
    private const string _dbConnectionString = "Host=localhost;Port=5433;Database=TestDb_ScopeService;Username=test;Password=Test";

    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScope _serviceScope;
    private readonly IScopeService _scopeService;
    private readonly IMediator _mediator;
    private readonly AppDbContext _appDbContext;

    public ScopeServiceTests()
    {
        ServiceCollection services = new();
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
        services.AddScopeService();
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(_dbConnectionString));

        _serviceProvider = services.BuildServiceProvider();
        _scopeService = _serviceProvider.GetService<IScopeService>();

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
    public async Task AppDbContextInsideMediatR_WhenUseConcurrencyWithScopeService_ShouldSuccess()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();
        string[] productNames = ["P1", "P2"];
        List<Task> tasks = [];

        // Act
        foreach (string productName in productNames)
        {
            tasks.Add(_scopeService.Run(new AddProductCommand { Name = productName }));
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
            _scopeService.FireAndForget(new AddProductCommand { Name = productName });
        }

        // Assert
        await Task.Delay(250);
        string[] assertProductNames = await _appDbContext.Products.Select(x => x.Name).ToArrayAsync();
        Assert.Equal(2, assertProductNames.Length);
        Assert.Equal(productNames.ToHashSet(), assertProductNames.ToHashSet());
    }

    [Fact]
    public async Task AppDbContext_WhenUseConcurrencyWithScopeService_ShouldSuccess()
    {
        // Arrange
        await _appDbContext.Products.ExecuteDeleteAsync();
        string[] productNames = ["P1", "P2"];

        // Act
        foreach (string productName in productNames)
        {
            _scopeService.FireAndForget(async scope =>
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