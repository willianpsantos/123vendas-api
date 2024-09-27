namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainSaveChangesAsyncService
    {
        ValueTask<int> SaveChangesAsync();
    }
}
