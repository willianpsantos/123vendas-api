namespace _123Vendas.Vendas.Domain.Models
{
    public class InsertUpdateOrDeleteSaleProductModel
    {
        public Guid? Id { get; set; }
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get => (Amount * Quantity) - Discount; }

        public bool IsDeleted { get; set; } = false;

        public DateTimeOffset? IncludedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get;set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
