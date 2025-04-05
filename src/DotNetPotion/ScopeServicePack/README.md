# ScopeService

The `ScopeService` service enables you to execute a task in a new thread within a new Service Provider scope.

<br/>

## Features

- Provides access to a new service provider scope within a new thread
- Supports **fire-and-forget** functionality
- Supports **MediatR** commands, queries, and notifications

<br/>

## Sample Use Cases

- When you need to use your **DbContext concurrently**
- When you want to execute a **background task** to avoid delaying the request scope completion
- When you want to publish MediatR notifications in a new scope

<br/>

## How to use

- For detailed usage examples, please refer to the tests in the `ScopeService` directory within the `tests` folder.

<br/>

### Inject the service

Register the service in your dependency injection container:

```csharp
services.AddScopeService();
```

<br/>

### Execute MediatoR Commands

Fire-and-forget:

```csharp
_scopeService.FireAndForget(new YourMediatRCommand());
```

Running multiple commands concurrently:

```csharp
List<Task> tasks = [];
tasks.Add(_scopeService.Run(new YourMediatRCommand1());
tasks.Add(_scopeService.Run(new YourMediatRCommand2());
await Task.WhenAll(tasks);
```

<br/>

### Access a new scope in a new thread

Run a task with access to the new scope:

```csharp
_scopeService.Run(async scope =>
{
    // Get services from the new scope
    IAppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

    // Your logic here...
});
```
