using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace FC.CodeFlix.Catalog.API.Configurations
{
    public static class UseCasesConfiguration
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(CreateCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DeleteCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetGenre).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetCastMember).Assembly));
            services.AddRepositories();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<ICastMemberRepository, CastMemberRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
        
    }
}
