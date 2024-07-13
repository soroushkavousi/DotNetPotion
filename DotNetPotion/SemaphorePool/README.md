# SemaphorePool

`SemaphorePool` provides key-based SemaphoreSlim instances for dynamic and reusable thread-locking mechanism, allowing configurable timeouts and logging thresholds to efficiently manage and monitor access to shared resources.

<br/>

## Key Features
- **Dynamic Locking with Keys:** Use dynamic parameters such as UserId for locking, allowing users to avoid waiting for locked code sections of other users.

- **Reusable Synchronization:** Utilize the same SemaphoreSlim across different locations by using a consistent key, ensuring consistent locking behavior.

- **Configurable Timeout:** Set a default timeout for waiting to enter a semaphore.

- **Automatic Retry**: If the timeout is reached, it attempts to create a new semaphore and retries the wait operation.

- **Logging Threshold:** Log warnings if waiting for a semaphore exceeds a specified duration, helping to identify performance bottlenecks.

- **Customizable parameters:** Configure your desired timeout and logging time threshold. SemaphorePool uses ILogger to log messages.

<br/>

## How to use

### Inject the service

Register the service in your dependency injection container:

```csharp
services.AddSemaphorePool();
```


#### Configuration

Configure the **default timeout** and **logging threshold** using the `SemaphorePoolOptions` class. These options can be set in your application’s configuration file or directly in code.

```csharp
services.AddSemaphorePool(options =>
{
    options.DefaultTimeout = TimeSpan.FromSeconds(30);
    options.DefaultLogTimeThreshold = TimeSpan.FromSeconds(5);
});
```

<br/>

### Examples

Lock a section of code separately for different users using dynamic parameters such as UserId:

```
string semaphoreKey = $"ModifyResourceA_{UserId}"; // Keys can be generated from a shared location such as SemaphoreKeys
SemaphoreSlim semaphore = await _semaphorePool.WaitAsync(semaphoreKey); 
try
{
    // Perform operations that require synchronized access to a shared resource
    await Task.Delay(1000); // Simulate work
}
finally
{
    semaphore.Release();
}
```