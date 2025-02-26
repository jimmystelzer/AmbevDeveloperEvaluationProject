using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");
            
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required");
            
        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required");
            
        RuleFor(x => x.BranchName)
            .NotEmpty()
            .WithMessage("Branch name is required");
            
        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Sale date is required");
            
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item");
            
        RuleForEach(x => x.Items).ChildRules(item => {
            item.RuleFor(i => i.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");
                
            item.RuleFor(i => i.ProductName)
                .NotEmpty()
                .WithMessage("Product name is required");
                
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0")
                .LessThanOrEqualTo(20)
                .WithMessage("Cannot sell more than 20 identical items");
                
            item.RuleFor(i => i.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");
        });
    }
}
