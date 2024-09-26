using _123Vendas.Vendas.Data.Base;

namespace _123Vendas.Vendas.Data.Entities
{
    public class Sale : Entity
    {        
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalerId { get; set; }

        public string SaleCode { get; set; } = "";
        public DateTimeOffset SaleDate { get;set; }
        public decimal Total {  get; set; }

        public bool? Canceled { get; set; }
        public DateTimeOffset? CanceledAt { get; set; }
        public Guid? CanceledBy { get; set; }

        public ICollection<SaleProduct>? Products { get; set; }
    }
}
