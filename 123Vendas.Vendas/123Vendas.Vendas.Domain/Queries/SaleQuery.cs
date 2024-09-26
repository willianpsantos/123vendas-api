namespace _123Vendas.Vendas.Domain.Queries
{
    public class SaleQuery : PageableQuery
    {
        public Guid? Id { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? SalerId { get; set; }

        public string? SaleCode { get; set; } = "";
        public DateTimeOffset? SaleDate { get; set; }
        public bool? Canceled { get; set; }
    }
}
