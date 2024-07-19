using DotNetPotion.SemaphorePool;
using DotNetPotion.Tests.SemaphorePool;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetPotion.Tests.ScopedTaskRunner;

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
    public async Task DecreaseUserBlanace_WithSemaphorePool_FastZeroBalanceResult()
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

        List<decimal> userBlanaces = [];
        foreach (int userId in userIds)
            userBlanaces.Add(await _userBalanceService.GetUserBlanaceAsync(userId));

        // Assert
        Assert.True(userBlanaces.All(userBalance => userBalance == 0));
    }

    [Fact]
    public async Task DecreaseUserBlanace_WithSharedSemaphoreSlim_SlowZeroBalanceResult()
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

        List<decimal> userBlanaces = [];
        foreach (int userId in userIds)
            userBlanaces.Add(await _userBalanceService.GetUserBlanaceAsync(userId));

        // Assert
        Assert.True(userBlanaces.All(x => x == 0));
    }
}