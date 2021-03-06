using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Speedo.Feature.SitecorePublisher
{
    public class SpeedoServiceConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<SpeedoConfiguration>();
        }
    }
}