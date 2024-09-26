using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.DB;
using _123Vendas.Vendas.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace _123Vendas.Vendas.Repositories
{
    public class SaleProductRepository : IRepository<SaleProduct>
    {
        private readonly SaleDbContext _context;

        public SaleProductRepository(SaleDbContext context) => _context = context;


        public async ValueTask<int> CountAsync(Expression<Func<SaleProduct, bool>>? query = default)
        {
            IQueryable<SaleProduct>? queryable = _context.SaleProducts;

            if (query is not null)
                queryable = queryable.Where(query);

            return await queryable.CountAsync();
        }


        public async IAsyncEnumerable<SaleProduct> GetAsync(Expression<Func<SaleProduct, bool>>? query = null, int page = 0, int pageSize = 0)
        {
            IQueryable<SaleProduct>? queryable = _context.SaleProducts;

            if (query is not null)
                queryable = queryable.Where(query);

            if (page > 0 && pageSize > 0)
                queryable = queryable.Skip((page - 1) * pageSize).Take(pageSize);

            await foreach (var item in queryable.AsNoTracking().AsAsyncEnumerable())
                yield return item;
        }

        public async ValueTask<SaleProduct?> GetByIdAsync(Guid id) => await 
            _context
                .SaleProducts
                .AsNoTracking()
                .FirstOrDefaultAsync(_ => _.Id == id);

        public async ValueTask<SaleProduct> InsertAsync(SaleProduct entity, Guid insertedBy)
        {
            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            entity.IncludedAt = DateTimeOffset.UtcNow;
            entity.IncludedBy = insertedBy;
            entity.IsDeleted = false;

            var entry = await _context.SaleProducts.AddAsync(entity);

            return entry.Entity;
        }

        public async ValueTask<SaleProduct> InsertAndSaveChangesAsync(SaleProduct entity, Guid insertedBy)
        {
            var insertedEntity = await InsertAsync(entity, insertedBy);

            await _context.SaveChangesAsync();

            return insertedEntity;
        }

        public SaleProduct Update(SaleProduct entity, Guid updatedBy)
        {
            if (entity.Id == Guid.Empty)
                throw new InvalidOperationException("Entity has no ID");

            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public async ValueTask<SaleProduct> UpdateAndSaveChangesAsync(SaleProduct entity, Guid updatedBy)
        {
            Update(entity, updatedBy);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy)
        {
            var affected = await _context
                .SaleProducts
                .Where(_ => _.Id == id)
                .ExecuteUpdateAsync(
                    _ => _.SetProperty(_ => _.IsDeleted, true)
                          .SetProperty(_ => _.DeletedAt, DateTimeOffset.UtcNow)
                          .SetProperty(_ => _.DeletedBy, deletedBy)
                );

            return affected > 0;
        }

        public async ValueTask<bool> DeleteAndSaveChangesAsync(Guid id, Guid deletedBy)
        {
            var deleted = await DeleteAsync(id, deletedBy);

            if (deleted)
                await _context.SaveChangesAsync();

            return deleted;
        }

        public async ValueTask<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
