namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainInsertAsyncService<TModel> where TModel : class
    {
        ValueTask<Guid> InsertAsync(TModel model, Guid insertedBy);
    }
}
