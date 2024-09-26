using _123Vendas.Vendas.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _123Vendas.Vendas.DB.EntityConfigurations
{
    public class SaleProductEntityConfiguration : IEntityTypeConfiguration<SaleProduct>
    {
        public void Configure(EntityTypeBuilder<SaleProduct> builder)
        {
            builder.ToTable("sale_products");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.SaleId).HasColumnName("sale_id").IsRequired();
            builder.Property(x => x.ProductId).HasColumnName("product_id").IsRequired();
            builder.Property(x => x.Canceled).HasColumnName("canceled");
            builder.Property(x => x.CanceledAt).HasColumnName("canceled_at");
            builder.Property(x => x.CanceledBy).HasColumnName("canceled_by");

            builder.Property(x => x.IsDeleted).HasDefaultValue(false).HasColumnName("deleted");

            builder.Property(x => x.IncludedAt).HasDefaultValue(DateTimeOffset.UtcNow).HasColumnName("included_at");
            builder.Property(x => x.IncludedBy).IsRequired().HasColumnName("included_by");

            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

            builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
            builder.Property(x => x.DeletedBy).HasColumnName("deleted_by");

            builder.HasOne(x => x.Sale).WithMany(x => x.Products);

            builder
                .OwnsOne(x => x.Amount)
                .Property(v => v.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder
                .OwnsOne(x => x.Amount)
                .Property(v => v.Amount)
                .HasColumnName("amount")
                .IsRequired();

            builder
                .OwnsOne(x => x.Amount)
                .Property(v => v.Discount)
                .HasColumnName("discount")
                .HasDefaultValue(0);
        }
    }
}
