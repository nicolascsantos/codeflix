using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Storage.Configuration;
using Google.Cloud.Storage.V1;
using StorageService = FC.CodeFlix.Catalog.Infra.Storage.Services;

namespace FC.CodeFlix.Catalog.API.Configurations
{
    public static class StorageConfiguration
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(_ => StorageClient.Create());
            services.Configure<StorageServiceOptions>(
                configuration.GetSection(StorageServiceOptions.ConfigurationSection
            ));
            services.AddTransient<IStorageService, StorageService.StorageService>();
            return services;
        }
    }
}
