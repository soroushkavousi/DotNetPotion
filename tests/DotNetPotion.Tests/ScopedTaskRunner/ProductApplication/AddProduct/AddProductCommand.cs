using MediatR;

namespace DotNetPotion.Tests.ScopedTaskRunner.ProductApplication.AddProduct;

public class AddProductCommand : IRequest
{
    public string Name { get; set; }
    public TimeSpan? ManualDelay { get; set; }
}