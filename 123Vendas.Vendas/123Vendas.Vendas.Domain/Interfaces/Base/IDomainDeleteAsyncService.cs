namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainDeleteAsyncService
    {
        ValueTask<bool> DeleteAsync(Guid id, Guid deletedBy);
    }
}
