using _123Vendas.Vendas.Domain.Models;
using FluentValidation;

namespace _123Vendas.Vendas.Domain.Validators
{
    public class InsertOrUpdateSaleProductModelValidator : AbstractValidator<InsertOrUpdateSaleProductModel>
    {
        public InsertOrUpdateSaleProductModelValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.ProductId).NotEmpty().NotEqual(Guid.Empty);
        }
    }
}
