namespace _123Vendas.Vendas.Domain.Interfaces
{
    public interface IDomainService<TModel> where TModel : class
    {
        ValueTask<IEnumerable<TModel>> GetAsync<TQuery>(TQuery? query = default) where TQuery : class;
        ValueTask<TModel?> GetByIdAsync(Guid id);
        ValueTask<TModel> InsertAsync(TModel model, Guid insertedBy);
        TModel Update(TModel model, Guid updatedBy);
        ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy);
        ValueTask<int> SaveChangesAsync();
    }
}
