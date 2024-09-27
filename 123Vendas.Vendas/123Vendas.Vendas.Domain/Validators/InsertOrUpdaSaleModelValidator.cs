using _123Vendas.Vendas.Domain.Models;
using FluentValidation;

namespace _123Vendas.Vendas.Domain.Validators
{
    public class InsertOrUpdaSaleModelValidator : AbstractValidator<InsertOrUpdateSaleModel>
    {
        public InsertOrUpdaSaleModelValidator() 
        {
            RuleFor(x => x.SaleCode).NotEmpty();
            RuleFor(x => x.SalerId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.BranchId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.CompanyId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.CustomerId).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(x => x.SaleDate).NotEmpty().GreaterThan(DateTimeOffset.MinValue).LessThan(DateTimeOffset.MaxValue);

            RuleFor(x => x.Products).NotNull().NotEmpty();
            RuleForEach(x => x.Products).SetValidator(new InsertOrUpdateSaleProductModelValidator());
        }
    }
}
