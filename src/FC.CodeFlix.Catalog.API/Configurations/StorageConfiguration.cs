using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Infra.Storage.Configuration;
using Google.Cloud.Storage.V1;
using Storage = FC.CodeFlix.Catalog.Infra.Storage.Services;
using FC.CodeFlix.Catalog.Infra.Storage.Enum;
using Amazon.S3;

namespace FC.CodeFlix.Catalog.API.Configurations
{
    public static class StorageConfiguration
    {
        public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(StorageServiceOptions.ConfigurationSection);
            services.Configure<StorageServiceOptions>(section);

            var provider = section.Get<StorageServiceOptions>()?.StorageProvider ?? StorageProviderType.AWS;

            if (provider == StorageProviderType.AWS)
            {
                services.AddDefaultAWSOptions(configuration.GetAWSOptions());
                services.AddAWSService<IAmazonS3>();
                services.AddTransient<IStorageService, Storage.AWSStorageService>();
            }
            else
            {
                services.AddScoped(_ => StorageClient.Create());
                services.AddTransient<IStorageService, Storage.StorageService>();
            }

            return services;
        }
    }
}
