using FC.CodeFlix.Catalog.API.Configurations.Policies;
using FC.CodeFlix.Catalog.API.FIlters;

namespace FC.CodeFlix.Catalog.API.Configurations
{
    public static class ControllersConfiguration
    {
        public static IServiceCollection AddAndConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers(options => 
                options.Filters.Add(typeof(APIGlobalExceptionFilter)))
                    .AddJsonOptions(x => x.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCasePolicy());
            services.AddDocumentation();
            return services;
        }

        private static IServiceCollection AddDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        public static WebApplication UseDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }
    }
}
