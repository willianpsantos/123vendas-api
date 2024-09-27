namespace _123Vendas.Vendas.Domain.Models
{
    public class InsertOrUpdateSaleModel
    {
        public InsertOrUpdateSaleModel()
        {
            
        }

        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalerId { get; set; }

        public string SaleCode { get; set; } = "";
        public DateTimeOffset SaleDate { get; set; }

        public IEnumerable<InsertOrUpdateSaleProductModel>? Products { get; set; }
    }
}
