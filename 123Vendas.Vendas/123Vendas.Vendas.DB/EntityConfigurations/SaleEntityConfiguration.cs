using _123Vendas.Vendas.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _123Vendas.Vendas.DB.EntityConfigurations
{
    public class SaleEntityConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("sales");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.SaleCode).IsRequired().HasMaxLength(100).HasColumnName("sale_code");
            builder.Property(x => x.BranchId).IsRequired().HasColumnName("banch_id");
            builder.Property(x => x.CompanyId).IsRequired().HasColumnName("company_id");
            builder.Property(x => x.CustomerId).IsRequired().HasColumnName("customer_id");
            builder.Property(x => x.SalerId).IsRequired().HasColumnName("saler_id");
            builder.Property(x => x.SaleDate).IsRequired().HasColumnName("sale_date");            
            builder.Property(x => x.Total).IsRequired().HasColumnName("total");

            builder.Property(x => x.IsDeleted).HasDefaultValue(false).HasColumnName("deleted");

            builder.Property(x => x.IncludedAt).HasDefaultValue(DateTimeOffset.UtcNow).HasColumnName("included_at");
            builder.Property(x => x.IncludedBy).IsRequired().HasColumnName("included_by");

            builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            builder.Property(x => x.UpdatedBy).HasColumnName("updated_by");

            builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");
            builder.Property(x => x.DeletedBy).HasColumnName("deleted_by");

            builder
                .HasMany(x => x.Products)
                .WithOne(x => x.Sale)
                .HasForeignKey(x => x.SaleId);
        }
    }
}
