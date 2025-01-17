using DotNetPotion.SemaphorePoolPack;

namespace DotNetPotion.Tests.SemaphorePoolTests;

public class UserBalanceService(ISemaphorePool semaphorePool)
{
    private readonly SemaphoreSlim _sharedSemaphoreSlim = new(1, 1);

    private readonly Dictionary<int, decimal> _userBalances = [];

    public async Task DecreaseUserBalanceWithSemaphorePoolAsync(int userId, decimal count)
    {
        SemaphoreSlim semaphore = await semaphorePool.WaitAsync($"User_{userId}");
        try
        {
            decimal currentUserBalance = await GetUserBalanceAsync(userId);
            if (currentUserBalance < count)
                return;

            await Task.Delay(6); // database delay simulation
            _userBalances[userId] = currentUserBalance - count;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task DecreaseUserBalanceWithSharedSemaphoreSlimAsync(int userId, decimal count)
    {
        await _sharedSemaphoreSlim.WaitAsync();
        try
        {
            decimal currentUserBalance = await GetUserBalanceAsync(userId);
            if (currentUserBalance < count)
                return;

            await Task.Delay(6); // database delay simulation
            _userBalances[userId] = currentUserBalance - count;
        }
        finally
        {
            _sharedSemaphoreSlim.Release();
        }
    }

    public Task SetUserBalance(int userId, decimal count)
    {
        _userBalances[userId] = count;
        return Task.CompletedTask;
    }

    public async Task<decimal> GetUserBalanceAsync(int userId)
    {
        await Task.Delay(4); // database delay simulation
        return _userBalances[userId];
    }
}