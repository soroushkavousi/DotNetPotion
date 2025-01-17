using MediatR;

namespace DotNetPotion.Tests.ScopedTaskRunnerTests.ProductApplication.AddProduct;

public class AddProductCommand : IRequest
{
    public required string Name { get; init; }
    public TimeSpan? ManualDelay { get; init; }
}