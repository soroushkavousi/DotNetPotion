# DotNetPotion

Ready to write some .NET code? **Drink this potion** to warm up and get started!

**DotNetPotion** offers ready-to-use tools to simplify and enhance your development experience, making your code more efficient and easier to manage.

# Installation (To-Do)

To install `DotNetPotion`, run the following command in the Package Manager Console:

```shell
dotnet add package DotNetPotion
```

or

```shell
Install-Package DotNetPotion
```

# Tools

1. [ScopedTaskRunner](#1-scopedtaskrunner)

---

## 1. ScopedTaskRunner

The `ScopedTaskRunner` service allows you to run a task in a new thread within **a new Service Provider scope** using `Task.Run`. It additionally provides functionality to run **MediatR** IRequest objects and **fire-and-forget** tasks effortlessly.

One use-case for this tools is **when you want to use your DbContext concurrently**.

### How to use

- Inject Service

Register the service in your dependency injection container:

```csharp
services.AddScopedTaskRunner();
```

<br/>

- Execute MediatoR Commands

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

- Access the New Scope

Run a task with access to the new scope:

```csharp
_scopedTaskRunner.Run(async scope =>
{
    // Get services from the new scope
    IAppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

    // Your logic here...
});
```

---

# Contributing

Contributions are welcome! Feel free to open an issue or submit a pull request.

# Contact

You can contact me via the email soroushkavousi.me@gmail.com.
