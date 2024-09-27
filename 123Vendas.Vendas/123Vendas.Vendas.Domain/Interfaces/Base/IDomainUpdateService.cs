namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IDomainUpdateService<TModel, TReturnModel> 
        where TModel : class
        where TReturnModel : class
    {
        TReturnModel Update(Guid id, TModel model, Guid updatedBy);
    }
}
