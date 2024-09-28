using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.Domain.Interfaces.Base;
using _123Vendas.Vendas.Domain.Queries;
using System.Linq.Expressions;

namespace _123Vendas.Vendas.Domain.QueryAdapters
{
    public class SaleProductQueryToExpressionAdapter : IQueryToExpressionAdapter<SaleProductQuery, SaleProduct>
    {
        public Expression<Func<SaleProduct, bool>>? ToExpression(SaleProductQuery? query)
        {
            if (query is null)
                return null;

            Expression<Func<SaleProduct, bool>> expr =
                _ => (query.SaleId == null || _.SaleId == query.SaleId) &&
                     (query.ProductId == null || _.ProductId == query.ProductId) &&

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
