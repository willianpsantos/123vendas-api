using _123Vendas.Vendas.DependencyInjection;
using _123Vendas.Vendas.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace _123Vendas.Vendas.DependencyInjection
{
    public delegate void ServiceCollectionDependenciesHandler(IServiceCollection services);

    public class ManualDependencyInjection
    {
        private ServiceProvider? _serviceProvider;
        private IConfiguration? _configuration;

        private bool _buildConfiguration = true;
        private string _appSettingPath = ApplicationConstants.AppSettingsFilename;

        internal ManualDependencyInjection()
        {
            
        }

        public ManualDependencyInjection BuildConfiguration(bool value)
        {
            _buildConfiguration = value;
            return this;
        }

        public ManualDependencyInjection AppSettingsPath(string value)
        {
            _appSettingPath = value;
            return this;
        }

        public IConfiguration? GetConfiguration() => _configuration;


        internal void Init(ServiceCollectionDependenciesHandler dependenciesHandler)
        {
            var serviceCollection = new ServiceCollection();

            if (_buildConfiguration)
            {
                var configBuilder = new ConfigurationBuilder();
                var config = configBuilder.AddJsonFile(_appSettingPath).Build();

                _configuration = config;
            }

            dependenciesHandler?.Invoke(serviceCollection);

            //serviceCollection
            //    .AddDomainDbContext(_configuration)
            //    .AddDomainRepositories()
            //    .AddDomainServices();

            _serviceProvider = serviceCollection.BuildServiceProvider();            
        }        

        public T? GetService<T>() where T : notnull
        {
            if (_serviceProvider is null)
                return default;

            var service = _serviceProvider.GetRequiredService<T>();

            return service;
        }
    }
}
