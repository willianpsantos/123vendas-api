using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.Domain.Interfaces;
using _123Vendas.Vendas.Domain.Models;
using _123Vendas.Vendas.Domain.Queries;

namespace _123Vendas.Vendas.Services
{
    public class SaleService : IDomainService<SaleModel>
    {
        private readonly IRepository<Sale> _repository;
        private readonly IRepository<SaleProduct> _productRepository;
        private readonly IQueryToExpressionAdapter<SaleQuery, Sale> _queryAdapter;

        public SaleService(
            IRepository<Sale> repository, 
            IRepository<SaleProduct> productRepository,
            IQueryToExpressionAdapter<SaleQuery, Sale> queryAdapter
        ) 
        {
            _repository = repository;
            _productRepository = productRepository;
            _queryAdapter = queryAdapter;
        }


        private SaleModel _ToModel(Sale entity) =>
            new SaleModel
            {
                BranchId = entity.BranchId,
                Canceled = entity.Canceled,
                CanceledAt = entity.CanceledAt,
                CanceledBy = entity.CanceledBy,
                CompanyId = entity.CompanyId,
                CustomerId = entity.CustomerId,
                Id = entity.Id,
                SaleCode = entity.SaleCode,
                SaleDate = entity.SaleDate,
                SalerId = entity.SalerId,
                Total = entity.Total,

                Products = entity?.Products?.Select(_ => new SaleProductModel
                {
                    Id = _.Id,
                    SaleId = _.SaleId,
                    Amount = _.Amount?.Amount ?? 0,
                    Discount = _.Amount?.Discount ?? 0,
                    Quantity = _.Amount?.Quantity ?? 0,
                    Canceled = _.Canceled,
                    CanceledAt = _.CanceledAt,
                    CanceledBy = _.CanceledBy,
                    ProductId = _.ProductId
                })
                    ?.ToArray() ?? Enumerable.Empty<SaleProductModel>()
            };

        private Sale _ToEntity(SaleModel model) =>
            new Sale
            {
                BranchId = model.BranchId,
                Canceled = model.Canceled,
                CanceledAt = model.CanceledAt,
                CanceledBy = model.CanceledBy,
                CompanyId = model.CompanyId,
                CustomerId = model.CustomerId,
                SaleCode = model.SaleCode,
                SaleDate = model.SaleDate,
                SalerId = model.SalerId,
                Total = model.Total,

                Products = model.Products?.Select(_ => new SaleProduct
                {
                    Amount = new Data.ValueObjects.ProductAmount(_.Quantity, _.Amount, _.Discount),
                    Canceled = _.Canceled,
                    CanceledAt= _.CanceledAt,
                    CanceledBy= _.CanceledBy,
                    Id = _.Id ?? Guid.Empty,
                    ProductId = _.ProductId,
                    SaleId = _.SaleId
                })
                .ToHashSet(),
            };

        public async ValueTask<IEnumerable<SaleModel>> GetAsync<TQuery>(TQuery? query = null) where TQuery : class
        {
            var salesQuery = query as SaleQuery;
            var expression = salesQuery is not null ? _queryAdapter.ToExpression(salesQuery) : null;
            var list = new HashSet<SaleModel>();

            await foreach (var entity in _repository.GetAsync(expression, salesQuery?.PageNumber ?? 0, salesQuery?.PageSize ?? 0))
                list.Add(_ToModel(entity));

            return list.ToArray();
        }

        public async ValueTask<SaleModel?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity is null ? null : _ToModel(entity);
        }

        public async ValueTask<SaleModel> InsertAsync(SaleModel model, Guid insertedBy)
        {
            var entity = _ToEntity(model);
            var insertedSale = await _repository.InsertAsync(entity, insertedBy);
            
            return _ToModel(insertedSale);
        }   

        public SaleModel Update(SaleModel model, Guid updatedBy)
        {
            var entity = _ToEntity(model);
            var updatedSale = _repository.Update(entity, updatedBy);
            
            return _ToModel(updatedSale);
        }

        public async ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy) => await _repository.DeleteAsync(id, deletedBy);

        public async ValueTask<int> SaveChangesAsync() => await _repository.SaveChangesAsync();

    }
}
