namespace _123Vendas.Vendas.Data.Base
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; } = false;

        public DateTimeOffset? IncludedAt { get; set; }
        public Guid? IncludedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
