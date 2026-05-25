using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FC.CodeFlix.Catalog.Application;
using FC.CodeFlix.Catalog.Application.EventHandlers;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.CodeFlix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory;
using FC.CodeFlix.Catalog.Application.UseCases.Genre.GetGenre;
using FC.CodeFlix.Catalog.Domain.Events;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Infra.Messaging.Configuration;
using FC.CodeFlix.Catalog.Infra.Messaging.Producer;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FC.CodeFlix.Catalog.API.Configurations
{
    public static class UseCasesConfiguration
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(CreateCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(DeleteCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetCategory).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetGenre).Assembly));
            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(GetCastMember).Assembly));
            services.AddRepositories();
            services.AddDomainEvents(configuration);
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IGenreRepository, GenreRepository>();
            services.AddTransient<IVideoRepository, VideoRepository>();
            services.AddTransient<ICastMemberRepository, CastMemberRepository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }

        private static IServiceCollection AddDomainEvents(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDomainEventPublisher, DomainEventPublisher>();
            services.AddTransient<IDomainEventHandler<VideoUploadedEvent>, SendToEncoderEventHandler>();
            services.Configure<RabbitMQConfiguration>(
                configuration.GetSection(RabbitMQConfiguration.CONFIGURATION_SECTION)
            );

            services.AddSingleton<Task<IConnection>>(async sp =>
            {
                RabbitMQConfiguration config = sp
                    .GetRequiredService<IOptions<RabbitMQConfiguration>>().Value;

                var factory = new ConnectionFactory
                {
                    HostName = config.Hostname!,
                    Port = config.Port,
                    UserName = config.Username!,
                    Password = config.Password!
                };

                return await factory.CreateConnectionAsync();
            });

            services.AddSingleton<ChannelManager>();

            services.AddTransient<Task<IMessageProducer>>(async sp =>
            {
                var channelManager = sp.GetRequiredService<ChannelManager>();
                var config = sp.GetRequiredService<IOptions<RabbitMQConfiguration>>();
                return new RabbitMQProducer(await channelManager.GetChannel(), config);
            });

            return services;
        }
    }
}
