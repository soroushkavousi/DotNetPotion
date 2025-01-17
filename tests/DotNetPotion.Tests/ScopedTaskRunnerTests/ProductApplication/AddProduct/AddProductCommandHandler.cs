using DotNetPotion.Tests.ScopedTaskRunnerTests.Data;
using MediatR;

namespace DotNetPotion.Tests.ScopedTaskRunnerTests.ProductApplication.AddProduct;

public class AddProductCommandHandler(AppDbContext appDbContext) : IRequestHandler<AddProductCommand>
{
    public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        Product product = new(request.Name);

        appDbContext.Products.Add(product);

        if (request.ManualDelay.HasValue)
            await Task.Delay(request.ManualDelay.Value, cancellationToken);

        await appDbContext.SaveChangesAsync(cancellationToken);
    }
}