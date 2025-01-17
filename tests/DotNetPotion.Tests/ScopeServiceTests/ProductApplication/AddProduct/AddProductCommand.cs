using MediatR;

namespace DotNetPotion.Tests.ScopeServiceTests.ProductApplication.AddProduct;

public class AddProductCommand : IRequest
{
    public required string Name { get; init; }
    public TimeSpan? ManualDelay { get; init; }
}