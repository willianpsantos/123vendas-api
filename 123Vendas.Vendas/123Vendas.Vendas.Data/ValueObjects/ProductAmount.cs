namespace _123Vendas.Vendas.Data.ValueObjects
{
    public record ProductAmount : IEquatable<ProductAmount>
    {
        public ProductAmount(decimal quantity, decimal amount, decimal discount = 0)
        {
            Quantity = quantity;
            Amount = amount;
            Discount = discount;
        }

        public decimal Quantity { get; }
        public decimal Amount { get; }
        public decimal Discount {  get; }        
        public decimal Total { get => CalculateTotal(); }

        private decimal CalculateTotal() => (Amount * Quantity) - Discount;

        public virtual bool Equals(ProductAmount? other)
        {
            if (other is null)
                return false;

            return Amount == other.Amount && Discount == other.Discount && Quantity == other.Quantity;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
