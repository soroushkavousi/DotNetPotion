using DotNetPotion.Tests.ScopedTaskRunner.Data;
using MediatR;

namespace DotNetPotion.Tests.ScopedTaskRunner.ProductApplication.AddProduct;

public class AddProductCommandHandler(AppDbContext appDbContext) : IRequestHandler<AddProductCommand>
{
    private AppDbContext _appDbContext = appDbContext;

    public async Task Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        Product product = new(request.Name);

        _appDbContext.Products.Add(product);

        if (request.ManualDelay.HasValue)
            await Task.Delay(request.ManualDelay.Value, cancellationToken);

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }
}