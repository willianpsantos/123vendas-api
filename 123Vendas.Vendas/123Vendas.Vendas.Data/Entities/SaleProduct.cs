using _123Vendas.Vendas.Data.Base;
using _123Vendas.Vendas.Data.ValueObjects;

namespace _123Vendas.Vendas.Data.Entities
{
    public class SaleProduct : Entity
    {
        public Guid SaleId { get; set; }
        public Sale? Sale { get; set; }

        public Guid ProductId { get; set; }
        public ProductAmount? Amount { get; set; }

        public bool? Canceled { get; set; }
        public DateTimeOffset? CanceledAt { get; set; }
        public Guid? CanceledBy { get; set; }
    }
}
