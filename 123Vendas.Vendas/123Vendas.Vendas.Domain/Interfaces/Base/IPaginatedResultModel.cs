namespace _123Vendas.Vendas.Domain.Interfaces.Base
{
    public interface IPaginatedResultModel<TData> where TData : class
    {
        int Count { get; set; }
        int PageNumber {  get; set; }
        int PageSize { get; set; }
        IEnumerable<TData>? Data { get; set; }
    }
}
