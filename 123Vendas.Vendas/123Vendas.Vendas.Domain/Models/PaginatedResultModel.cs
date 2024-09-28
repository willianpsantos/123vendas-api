using _123Vendas.Vendas.Domain.Interfaces.Base;

namespace _123Vendas.Vendas.Domain.Models
{
    public class PaginatedResultModel<TData> : IPaginatedResultModel<TData> where TData : class
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<TData>? Data { get; set; }
    }
}
