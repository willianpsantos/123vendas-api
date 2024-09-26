using System.Linq.Expressions;

namespace _123Vendas.Vendas.Domain.Interfaces
{
    public interface IQueryToExpressionAdapter<TQuery, TEntity> 
        where TEntity : class 
        where TQuery : class
    {
        Expression<Func<TEntity, bool>> ToExpression(TQuery query);
    }
}
