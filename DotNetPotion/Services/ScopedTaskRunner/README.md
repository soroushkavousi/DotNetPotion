# DotNetPotion

[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-blue?logo=nuget)](https://www.nuget.org/packages/DotNetPotion)

<br/>

# ScopedTaskRunner

The `ScopedTaskRunner` service allows you to run a task in a new thread within **a new Service Provider scope**. 

- One use-case for this tools is **when you want to use your DbContext concurrently**.

- It additionally provides functionality to run **MediatR** IRequest objects and **fire-and-forget** tasks effortlessly.

<br/>

## How to use

### Inject the service

Register the service in your dependency injection container:

```csharp
services.AddScopedTaskRunner();
```

<br/>

### Execute MediatoR Commands

Fire-and-forget:

```csharp
_scopedTaskRunner.FireAndForget(new YourMediatRCommand());
```

Running multiple commands concurrently:

```csharp
List<Task> tasks = [];
tasks.Add(_scopedTaskRunner.Run(new YourMediatRCommand1());
tasks.Add(_scopedTaskRunner.Run(new YourMediatRCommand2());
await Task.WhenAll(tasks);
```

<br/>

### Access a new scope in a new thread

Run a task with access to the new scope:

```csharp
_scopedTaskRunner.Run(async scope =>
{
    // Get services from the new scope
    IAppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

    // Your logic here...
});
```
