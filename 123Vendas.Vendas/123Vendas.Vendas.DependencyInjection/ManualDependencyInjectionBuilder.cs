using _123Vendas.Vendas.Domain;

namespace _123Vendas.Vendas.DependencyInjection
{
    public class ManualDependencyInjectionBuilder
    {        
        private bool _buildConfiguration = true;
        private string _appSettingPath = ApplicationConstants.AppSettingsFilename;

        public ManualDependencyInjectionBuilder()
        {
            
        }

        public ManualDependencyInjectionBuilder BuildConfiguration(bool value)
        {
            _buildConfiguration = value;
            return this;
        }

        public ManualDependencyInjectionBuilder AppSettingsPath(string value)
        {
            _appSettingPath = value;
            return this;
        }

        public ManualDependencyInjection Build(ServiceCollectionDependenciesHandler dependenciesHandler)
        {
            var di = new ManualDependencyInjection();

            di.AppSettingsPath(_appSettingPath)
              .BuildConfiguration(_buildConfiguration)
              .Init(dependenciesHandler);

            return di;
        }
    }
}
