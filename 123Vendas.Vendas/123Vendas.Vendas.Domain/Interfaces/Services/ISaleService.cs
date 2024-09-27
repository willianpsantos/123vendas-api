using _123Vendas.Vendas.Domain.Interfaces.Base;
using _123Vendas.Vendas.Domain.Models;
using _123Vendas.Vendas.Domain.Queries;

namespace _123Vendas.Vendas.Domain.Interfaces.Services
{
    public interface ISaleService :
        IDomainGetAsyncService<SaleQuery, SaleModel>,
        IDomainGetByIdAsyncService<SaleModel>,
        IDomainInsertAsyncService<InsertOrUpdateSaleModel>,
        IDomainUpdateService<InsertOrUpdateSaleModel, SaleModel>,
        IDomainDeleteAsyncService,
        IDomainCancelAsyncService,
        IDomainSaveChangesAsyncService
    {
    }
}
