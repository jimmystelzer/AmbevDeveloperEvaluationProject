using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public record DeleteSaleCommand : IRequest<bool>
{
    
    public Guid Id { get; }

    public DeleteSaleCommand(Guid id)
    {
        Id = id;
    }
}
