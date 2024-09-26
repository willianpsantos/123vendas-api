using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.DB.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace _123Vendas.Vendas.DB
{
    public class SaleDbContext : DbContext
    {
        public SaleDbContext(DbContextOptions<SaleDbContext> options) : base(options)
        {

        }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleProduct> SaleProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SaleEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SaleProductEntityConfiguration());
        }
    }
}
