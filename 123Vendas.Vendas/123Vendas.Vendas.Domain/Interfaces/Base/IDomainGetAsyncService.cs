namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainGetAsyncService<TQuery, TModel>
        where TModel : class
        where TQuery : class
    {
        ValueTask<int> CountAsync(TQuery? query = default);
        ValueTask<IEnumerable<TModel>> GetAsync(TQuery? query = default);
    }
}
