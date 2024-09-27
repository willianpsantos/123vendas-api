namespace _123Vendas.Vendas.Domain.Events
{
    public class SaleProductAddedOrUpdateEvent
    {
        public Guid? Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
    }
}
