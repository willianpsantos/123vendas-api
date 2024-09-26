using System.Linq.Expressions;

namespace _123Vendas.Vendas.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<int> CountAsync(Expression<Func<TEntity, bool>>? query = default);

        IAsyncEnumerable<TEntity> GetAsync(Expression<Func<TEntity, bool>>? query = default, int page = 0, int pageSize = 0);
        ValueTask<TEntity?> GetByIdAsync(Guid id);
        
        ValueTask<TEntity> InsertAsync(TEntity entity, Guid insertedBy);
        ValueTask<TEntity> InsertAndSaveChangesAsync(TEntity entity, Guid insertedBy);
        TEntity Update(TEntity entity, Guid updatedBy);
        ValueTask<TEntity> UpdateAndSaveChangesAsync(TEntity entity, Guid updatedBy);
        ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy);
        ValueTask<bool> DeleteAndSaveChangesAsync(Guid id, Guid deletedBy);
        ValueTask<int> SaveChangesAsync();
    }
}
