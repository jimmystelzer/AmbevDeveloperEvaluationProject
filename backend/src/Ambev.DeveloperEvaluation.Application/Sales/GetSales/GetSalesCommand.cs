using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public record GetSalesCommand : IRequest<GetSalesResult>
{
    public int Page { get; }

    public int PageSize { get; }

    public GetSalesCommand(int page, int pageSize)
    {
        Page = page > 0 ? page : 1;
        PageSize = pageSize > 0 ? pageSize : 10;
    }
}
