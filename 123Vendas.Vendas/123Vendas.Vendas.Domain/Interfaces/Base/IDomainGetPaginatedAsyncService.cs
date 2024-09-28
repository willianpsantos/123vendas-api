namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainGetPaginatedAsyncService<TQuery, TModel>
        where TModel : class
        where TQuery : class
    {
        ValueTask<IPaginatedResultModel<TModel>> GetPaginatedAsync(int page, int pageSize, TQuery? query = default);
    }
}
