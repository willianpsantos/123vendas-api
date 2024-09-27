namespace _123Vendas.Vendas.Domain.Events
{
    public class SaleCanceledEvent
    {
        public Guid SaleId { get; set; }
        public Guid CanceledBy { get; set; }
    }
}
