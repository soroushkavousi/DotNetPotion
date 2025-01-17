using DotNetPotion.SemaphorePoolPack;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.Tests.SemaphorePoolTests;

public class SemaphorePoolTests
{
    private readonly UserBalanceService _userBalanceService;

    public SemaphorePoolTests()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddSemaphorePool();
        services.AddSingleton<UserBalanceService>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _userBalanceService = serviceProvider.GetService<UserBalanceService>();
    }

    [Fact]
    public async Task DecreaseUserBalance_WithSemaphorePool_FastZeroBalanceResult()
    {
        // Arrange
        int[] userIds = [1, 2, 3, 4, 5];
        foreach (int userId in userIds)
            await _userBalanceService.SetUserBalance(userId, 10);

        // Act
        List<Task> tasks = [];
        foreach (int i in Enumerable.Range(1, 20))
        {
            foreach (int userId in userIds)
                tasks.Add(_userBalanceService.DecreaseUserBalanceWithSemaphorePoolAsync(userId, 1));
        }

        await Task.WhenAll(tasks);

        List<decimal> userBalances = [];
        foreach (int userId in userIds)
            userBalances.Add(await _userBalanceService.GetUserBalanceAsync(userId));

        // Assert
        Assert.True(userBalances.All(userBalance => userBalance == 0));
    }

    [Fact]
    public async Task DecreaseUserBalance_WithSharedSemaphoreSlim_SlowZeroBalanceResult()
    {
        // Arrange
        int[] userIds = [1, 2, 3, 4, 5];
        foreach (int userId in userIds)
            await _userBalanceService.SetUserBalance(userId, 10);

        // Act
        List<Task> tasks = [];
        foreach (int i in Enumerable.Range(1, 20))
        {
            foreach (int userId in userIds)
                tasks.Add(_userBalanceService.DecreaseUserBalanceWithSharedSemaphoreSlimAsync(userId, 1));
        }

        await Task.WhenAll(tasks);

        List<decimal> userBalances = [];
        foreach (int userId in userIds)
            userBalances.Add(await _userBalanceService.GetUserBalanceAsync(userId));

        // Assert
        Assert.True(userBalances.All(x => x == 0));
    }
}