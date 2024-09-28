using _123Vendas.Vendas.Data.Base;
using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.Domain.Interfaces.Base;
using _123Vendas.Vendas.Domain.Interfaces.Services;
using _123Vendas.Vendas.Domain.Models;
using _123Vendas.Vendas.Domain.Queries;

namespace _123Vendas.Vendas.Services
{
    public class SaleService : ISaleService
    {
        private readonly IRepository<Sale> _repository;
        private readonly IQueryToExpressionAdapter<SaleQuery, Sale> _queryAdapter;

        public SaleService(
            IRepository<Sale> repository,
            IQueryToExpressionAdapter<SaleQuery, Sale> queryAdapter
        ) 
        {
            _repository = repository;
            _queryAdapter = queryAdapter;
        }


        private InsertOrUpdateSaleModel _ToInsertOrUpdateModel(Sale entity) =>
            new InsertOrUpdateSaleModel
            {
                BranchId = entity.BranchId,
                CompanyId = entity.CompanyId,
                CustomerId = entity.CustomerId,                
                SaleCode = entity.SaleCode,
                SaleDate = entity.SaleDate,
                SalerId = entity.SalerId,                

                Products = entity?.Products?.Select(_ => new InsertUpdateOrDeleteSaleProductModel
                {
                    Id = _.Id != Guid.Empty ? _.Id : null,
                    SaleId = _.SaleId,
                    ProductId = _.ProductId,

                    Amount = _.Amount?.Amount ?? 0,
                    Discount = _.Amount?.Discount ?? 0,
                    Quantity = _.Amount?.Quantity ?? 0,

                    IsDeleted = _.IsDeleted,

                    IncludedAt = _.IncludedAt,
                    UpdatedAt = _.UpdatedAt,
                    DeletedAt = _.DeletedAt
                })?
                .ToHashSet()
            };

        private SaleModel _ToListModel(Sale entity) =>
            new SaleModel
            {
                Id = entity.Id,
                SalerId = entity.SalerId,
                BranchId = entity.BranchId,
                CompanyId = entity.CompanyId,
                CustomerId = entity.CustomerId,

                SaleCode = entity.SaleCode,
                SaleDate = entity.SaleDate,
                Total = entity.Total,

                Canceled = entity.Canceled,
                CanceledAt = entity.CanceledAt,
                IncludedAt = entity.IncludedAt,
                UpdatedAt = entity.UpdatedAt,
                DeletedAt = entity.DeletedAt,                

                Products = entity?.Products?.Select(_ => new SaleProductModel
                {
                    Id = _.Id != Guid.Empty ? _.Id : null,
                    SaleId = _.SaleId,
                    ProductId = _.ProductId,

                    Amount = _.Amount?.Amount ?? 0,
                    Discount = _.Amount?.Discount ?? 0,
                    Quantity = _.Amount?.Quantity ?? 0,
                    Total = _.Amount?.Total ?? 0,

                    Canceled = _.Canceled,

                    CanceledAt = _.CanceledAt,
                    IncludedAt = _.IncludedAt,
                    UpdatedAt = _.UpdatedAt,
                    DeletedAt = _.DeletedAt
                })?
                .ToArray() ?? Enumerable.Empty<SaleProductModel>()
            };

        private Sale _ToEntity(SaleModel model) =>
            new Sale
            {
                Id = model.Id ?? Guid.Empty,
                BranchId = model.BranchId,
                CompanyId = model.CompanyId,
                CustomerId = model.CustomerId,
                SalerId = model.SalerId,

                Canceled = model.Canceled,
                CanceledAt = model.CanceledAt,
                IncludedAt = model.IncludedAt,
                UpdatedAt = model.UpdatedAt,
                DeletedAt = model.DeletedAt,

                SaleCode = model.SaleCode,
                SaleDate = model.SaleDate,                

                Products = model.Products?.Select(_ => new SaleProduct
                {
                    Id = _.Id ?? Guid.Empty,
                    ProductId = _.ProductId,
                    SaleId = _.SaleId,

                    Amount = new Data.ValueObjects.ProductAmount(_.Quantity, _.Amount, _.Discount),
                    
                    IsDeleted = false,
                    Canceled = _.Canceled,

                    CanceledAt = _.CanceledAt,
                    IncludedAt = _.IncludedAt,
                    UpdatedAt = _.UpdatedAt,
                    DeletedAt = _.DeletedAt
                })?
                .ToHashSet(),
            };

        private Sale _ToEntity(InsertOrUpdateSaleModel model, Guid? id = null) =>
           new Sale
           {
               Id = id ?? Guid.Empty,
               BranchId = model.BranchId,               
               CompanyId = model.CompanyId,
               CustomerId = model.CustomerId,
               SalerId = model.SalerId,

               SaleCode = model.SaleCode,
               SaleDate = model.SaleDate,

               Products = model.Products?.Select(_ => new SaleProduct
               {
                   Id = _.Id ?? Guid.Empty,
                   ProductId = _.ProductId,
                   SaleId = _.SaleId,

                   Amount = new Data.ValueObjects.ProductAmount(_.Quantity, _.Amount, _.Discount),                   

                   Canceled = false,
                   IsDeleted = _.IsDeleted,

                   IncludedAt = _.IncludedAt,
                   UpdatedAt = _.UpdatedAt,
                   DeletedAt = _.DeletedAt
               })
               .ToHashSet(),
           };


        public async ValueTask<int> CountAsync(SaleQuery? query = null)
        {
            var salesQuery = query as SaleQuery;
            var expression = salesQuery is not null ? _queryAdapter.ToExpression(salesQuery) : null;
            
            return await _repository.CountAsync(expression);
        }

        public async ValueTask<IEnumerable<SaleModel>> GetAsync(SaleQuery? query = null)
        {
            var expression = _queryAdapter?.ToExpression(query);
            var list = new HashSet<SaleModel>();

            await foreach (var entity in _repository.GetAsync(expression))
                list.Add(_ToListModel(entity));

            return list;
        }

        public async ValueTask<IPaginatedResultModel<SaleModel>> GetPaginatedAsync(int page, int pageSize, SaleQuery? query = null)
        {
            var expression = _queryAdapter?.ToExpression(query);
            var list = new HashSet<SaleModel>();

            await foreach (var entity in _repository.GetAsync(expression, page, pageSize))
                list.Add(_ToListModel(entity));

            var count = await _repository.CountAsync(expression);

            return new PaginatedResultModel<SaleModel>
            {
                Count = count,
                PageSize = pageSize,
                PageNumber = page,
                Data = list
            };
        }

        public async ValueTask<SaleModel?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity is null ? null : _ToListModel(entity);
        }

        public async ValueTask<Guid> InsertAsync(InsertOrUpdateSaleModel model, Guid insertedBy)
        {
            var entity = _ToEntity(model);
            var insertedSale = await _repository.InsertAsync(entity, insertedBy);            
            return insertedSale.Id;
        }   

        public SaleModel Update(Guid id, InsertOrUpdateSaleModel model, Guid updatedBy)
        {
            var entity = _ToEntity(model, id);
            var updatedSale = _repository.Update(entity, updatedBy);
            
            return _ToListModel(updatedSale);
        }

        public async ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy) => await _repository.DeleteAsync(id, deletedBy);

        public async ValueTask<bool> CancelAsync(Guid id, Guid canceledBy) => await _repository.CancelAsync(id, canceledBy);

        public async ValueTask<int> SaveChangesAsync() => await _repository.SaveChangesAsync();

    }
}
