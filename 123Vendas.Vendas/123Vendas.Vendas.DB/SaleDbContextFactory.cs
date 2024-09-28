using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace _123Vendas.Vendas.DB
{
    public class SaleDbContextFactory : IDesignTimeDbContextFactory<SaleDbContext>
    {
        public SaleDbContext CreateDbContext(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            var contextBuilder = new DbContextOptionsBuilder<SaleDbContext>();

            var config = configBuilder.AddJsonFile("sharedsettings.json").Build();
            var connectionString = config.GetConnectionString("SalesDb");

            contextBuilder.UseSqlServer(connectionString);

            return new SaleDbContext(contextBuilder.Options);
        }
    }
}
