using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.DB;
using _123Vendas.Vendas.Domain;
using _123Vendas.Vendas.Domain.Adapters;
using _123Vendas.Vendas.Domain.Interfaces.Base;
using _123Vendas.Vendas.Domain.Interfaces.Services;
using _123Vendas.Vendas.Domain.Models;
using _123Vendas.Vendas.Domain.Queries;
using _123Vendas.Vendas.Domain.Validators;
using _123Vendas.Vendas.Repositories;
using _123Vendas.Vendas.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace _123Vendas.Vendas.DependencyInjection
{
    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainOpenApiSettings(this IServiceCollection services)
        {
            return services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen();
                //.AddSwaggerGen(options =>
                //{
                //    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //    {
                //        Description = "JWT Bearer Authorization Header",
                //        Name = "Authorization",
                //        In = ParameterLocation.Header,
                //        Type = SecuritySchemeType.ApiKey,
                //        Scheme = "Bearer"
                //    });

                //    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //    {
                //        {
                //            new OpenApiSecurityScheme
                //            {
                //                Reference = new OpenApiReference
                //                {
                //                    Type = ReferenceType.SecurityScheme,
                //                    Id = "Bearer"
                //                },

                //                Scheme = "oauth2",
                //                Name = "Bearer",
                //                In = ParameterLocation.Header
                //            },

                //            new List<string>()
                //        }
                //    });
                //});
        }

        public static IServiceCollection AddDomainSqlServerDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<SaleDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString(ApplicationConstants.DatabaseConnectionStringName);
                options.UseSqlServer(connectionString);
            });
        }

        public static IServiceCollection AddDomainInMemoryDbContext(this IServiceCollection services)
        {
            return services.AddDbContext<SaleDbContext>(options =>
            {
                options.UseInMemoryDatabase(ApplicationConstants.InMemoryDatabaseName, options =>
                {
                    options.EnableNullChecks();
                });
            });
        }

        public static IServiceCollection AddDomainQueryToExpressionAdapters(this IServiceCollection services)
        {
            return services
                .AddScoped<IQueryToExpressionAdapter<SaleQuery, Sale>, SaleQueryToExpressionAdapter>();
        }

        public static IServiceCollection AddDomainRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<IRepository<Sale>, SaleRepository>()
                .AddScoped<IRepository<SaleProduct>, SaleProductRepository>();
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            return services
                .AddScoped<ISaleService, SaleService>();
        }

        public static IServiceCollection AddDomainModelValidators(this IServiceCollection services)
        {
            return services
                .AddScoped<IValidator<InsertOrUpdateSaleModel>, InsertOrUpdaSaleModelValidator>()
                .AddScoped<IValidator<InsertOrUpdateSaleProductModel>, InsertOrUpdateSaleProductModelValidator>();
        }
    }
}
