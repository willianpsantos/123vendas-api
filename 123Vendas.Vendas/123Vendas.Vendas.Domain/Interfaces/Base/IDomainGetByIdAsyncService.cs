namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainGetByIdAsyncService<TModel> where TModel : class
    {
        ValueTask<TModel?> GetByIdAsync(Guid id);
    }
}
