using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.DB;
using _123Vendas.Vendas.Domain.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace _123Vendas.Vendas.Repositories
{
    public class SaleRepository : IRepository<Sale>
    {
        private readonly SaleDbContext _context;

        public SaleRepository(SaleDbContext context) => _context = context;

        public async ValueTask<int> CountAsync(Expression<Func<Sale, bool>>? query = default)
        {
            IQueryable<Sale>? queryable =
                (query is null)
                    ? _context.Sales.Include(_ => _.Products).Where(_ => !_.IsDeleted)
                    : _context.Sales.Include(_ => _.Products).Where(query);
            
            return await queryable.CountAsync();
        }

        public async IAsyncEnumerable<Sale> GetAsync(Expression<Func<Sale, bool>>? query = null, int page = 0, int pageSize = 0)
        {
            IQueryable<Sale>? queryable =
                (query is null)
                    ? _context.Sales.Include(_ => _.Products!.Where(p => !p.IsDeleted)).Where(_ => !_.IsDeleted)
                    : _context.Sales.Include(_ => _.Products!.Where(p => !p.IsDeleted)).Where(query);

            if (page > 0 && pageSize > 0)
            {
                queryable =
                    queryable
                        .OrderByDescending(_ => _.IncludedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize);
            }

            await foreach (var item in queryable.AsNoTracking().AsAsyncEnumerable())
                yield return item;
        }

        public async ValueTask<Sale?> GetByIdAsync(Guid id) => await 
            _context
                .Sales
                .Include(_ => _.Products!.Where(p => !p.IsDeleted))
                .AsNoTracking()
                .FirstOrDefaultAsync(_ => _.Id == id);

        public async ValueTask<Sale> InsertAsync(Sale entity, Guid insertedBy)
        {
            if (entity.Products is null or { Count: 0 })
                throw new InvalidOperationException("Entity has no products");

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            entity.IncludedAt = DateTimeOffset.UtcNow;
            entity.IncludedBy = insertedBy;
            entity.IsDeleted = false;

            if (entity.Products is not null && entity.Products.Count > 0)
            {
                foreach (var prod in entity.Products)
                {
                    prod.Id = Guid.NewGuid();
                    prod.IncludedBy = insertedBy;
                    prod.IncludedAt = entity.IncludedAt;
                }

                entity.Total = entity.Products.Where(_ => !_.IsDeleted).Sum(_ => _.Amount?.Total ?? 0);
            }

            var entry = await _context.Sales.AddAsync(entity);

            return entry.Entity;
        }

        public async ValueTask<Sale> InsertAndSaveChangesAsync(Sale entity, Guid insertedBy)
        {
            var insertedEntity = await InsertAsync(entity, insertedBy);

            await _context.SaveChangesAsync();

            return insertedEntity;
        }

        public Sale Update(Sale entity, Guid updatedBy)
        {
            if (entity.Id == Guid.Empty)
                throw new InvalidOperationException("Entity has no ID");

            if (entity.Products is null or { Count: 0 })
                throw new InvalidOperationException("Entity has no products");

            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedBy = updatedBy;

            var trackedSale =
               _context
                   .ChangeTracker
                   .Entries<Sale>()?
                   .FirstOrDefault(_ => _.Entity?.Id == entity.Id);

            var trackedProducts =
                _context
                   .ChangeTracker
                   .Entries<SaleProduct>()?
                   .Where(_ => _.Entity?.SaleId == entity.Id)?
                   .ToArray();

            if (entity.Products is not null && entity.Products.Count > 0)
            {
                foreach (var prod in entity.Products)
                {
                    var inserting = prod.Id == Guid.Empty || prod.IncludedAt is null;                    

                    if (inserting)
                    {
                        prod.Id = prod.Id == Guid.Empty ? Guid.NewGuid() : prod.Id;
                        prod.IncludedAt = entity.UpdatedAt;
                        prod.IncludedBy = updatedBy;

                        continue;
                    }

                    if(prod.IsDeleted)
                    {
                        prod.DeletedAt = entity.UpdatedAt;
                        prod.DeletedBy = entity.DeletedBy;
                    }
                    else
                    {
                        prod.UpdatedAt = entity.UpdatedAt;
                        prod.UpdatedBy = updatedBy;
                    }

                    EntityEntry<SaleProduct>? trackedProd = trackedProducts?.FirstOrDefault(p => p.Entity.Id == prod.Id);

                    if (trackedProd is null)
                        _context.Entry(prod).State = EntityState.Modified;
                    else
                        trackedProd.CurrentValues.SetValues(prod);
                }

                entity.Total = 
                    entity
                        .Products
                        .Where(_ => !_.IsDeleted)
                        .Sum(_ => _.Amount?.Total ?? 0);
            }

            if (trackedSale is null)
                _context.Entry(entity).State = EntityState.Modified;
            else
                trackedSale.CurrentValues.SetValues(entity);

            return entity;
        }

        public async ValueTask<Sale> UpdateAndSaveChangesAsync(Sale entity, Guid updatedBy)
        {
            Update(entity, updatedBy);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy)
        {
            var affected = await _context
                .Sales
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

        public async ValueTask<bool> CancelAsync(Guid id, Guid canceledBy)
        {
            var affected = await _context
                .Sales
                .Where(_ => _.Id == id)
                .ExecuteUpdateAsync(
                    _ => _.SetProperty(_ => _.Canceled, true)
                          .SetProperty(_ => _.CanceledAt, DateTimeOffset.UtcNow)
                          .SetProperty(_ => _.CanceledBy, canceledBy)
                );

            return affected > 0;
        }

        public async ValueTask<bool> CancelAndSaveChangesAsync(Guid id, Guid canceledBy)
        {
            var canceled = await CancelAsync(id, canceledBy);

            if (canceled)
                await _context.SaveChangesAsync();

            return canceled;

        }

        public async ValueTask<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
