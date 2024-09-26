namespace _123Vendas.Vendas.Domain.Queries
{
    public abstract class PageableQuery
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
