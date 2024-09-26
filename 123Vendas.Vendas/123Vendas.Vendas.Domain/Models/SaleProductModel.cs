namespace _123Vendas.Vendas.Domain.Models
{
    public class SaleProductModel
    {
        public Guid? Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }

        public bool? Canceled { get; set; }
        public DateTimeOffset? CanceledAt { get; set; }
        public Guid? CanceledBy { get; set; }
    }
}
