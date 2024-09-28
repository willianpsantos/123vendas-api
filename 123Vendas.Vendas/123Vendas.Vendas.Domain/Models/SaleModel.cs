namespace _123Vendas.Vendas.Domain.Models
{
    public class SaleModel
    {
        public SaleModel()
        {
            
        }

        public Guid? Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalerId { get; set; }

        public string SaleCode { get; set; } = "";
        public DateTimeOffset SaleDate { get; set; }
        public decimal Total { get; set; }

        public bool? Canceled { get; set; }

        public DateTimeOffset? IncludedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public DateTimeOffset? CanceledAt { get; set; }

        public IEnumerable<SaleProductModel>? Products { get; set; }
    }
}
