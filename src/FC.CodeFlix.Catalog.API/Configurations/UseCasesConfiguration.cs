using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;

namespace FC.CodeFlix.Catalog.API.Configurations
{
    public static class UseCasesConfiguration
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(CreateCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DeleteCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetCategory).Assembly));
            services.AddRepositories();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
        
    }
}
