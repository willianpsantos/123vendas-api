namespace _123Vendas.Vendas.Domain.Queries
{
    public class SaleProductQuery : PageableQuery
    {
        public Guid? SaleId { get; set; }
        public Guid? ProductId { get; set; }
        public bool? Canceled { get; set; }
    }
}
