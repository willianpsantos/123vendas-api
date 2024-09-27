﻿namespace _123Vendas.Vendas.Domain.Events
{
    public class SaleCreatedEvent
    {
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SalerId { get; set; }

        public string SaleCode { get; set; } = "";
        public DateTimeOffset SaleDate { get; set; }

        public Guid CreatedBy { get; set; }

        public IEnumerable<SaleProductAddedOrUpdateEvent>? Products { get; set; }
    }
}
