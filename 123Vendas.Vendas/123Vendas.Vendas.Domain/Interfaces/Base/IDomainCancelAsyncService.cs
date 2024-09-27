namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainCancelAsyncService
    {
        ValueTask<bool> CancelAsync(Guid id, Guid canceledBy);
    }
}
