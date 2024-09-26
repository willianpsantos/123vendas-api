using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.Domain.Interfaces;
using _123Vendas.Vendas.Domain.Queries;
using System.Linq.Expressions;

namespace _123Vendas.Vendas.Domain.Adapters
{
    public class SaleQueryToExpressionAdapter : IQueryToExpressionAdapter<SaleQuery, Sale>
    {
        public Expression<Func<Sale, bool>> ToExpression(SaleQuery query)
        {
            if (query.Id.HasValue && query.Id != Guid.Empty)
                return _ => _.Id == query.Id;

            Expression<Func<Sale, bool>> expr =
                _ => (query.CompanyId == null || _.CompanyId == query.CompanyId) &&
                     (query.BranchId == null || _.BranchId == query.BranchId) &&
                     (query.CustomerId == null || _.CustomerId == query.CustomerId) &&
                     (query.SalerId == null || _.SalerId == query.SalerId) &&
                     (string.IsNullOrEmpty(query.SaleCode) || _.SaleCode == query.SaleCode) &&
                     (query.SaleDate == null || _.SaleDate == query.SaleDate) &&

                     (
                        query.Canceled == null ||
                        (
                            (query.Canceled == true && _.Canceled == true) ||
                            (query.Canceled == false && _.Canceled == false)
                        )
                     )

                     &&

                     _.IsDeleted == false;

            return expr;
        }
    }
}
