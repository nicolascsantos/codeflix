using FC.CodeFlix.Catalog.Application;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using UnitOfWorkInfra = FC.Codeflix.Catalog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.UnitOfWork
{
    [Collection(nameof(UnitOfWorkTestFixture))]
    public class UnitOfWorkTest : IClassFixture<UnitOfWorkTestFixture>
    {
        private readonly UnitOfWorkTestFixture _fixture;

        public UnitOfWorkTest(UnitOfWorkTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Commit))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Commit()
        {
            var dbContext = _fixture.CreateDbContext();
            var exampleCategoriesList = _fixture.GetExampleCategoriesList();
            var categoryWithEvent = exampleCategoriesList.First();
            var @event = new DomainEventFake();
            categoryWithEvent.RaiseEvent(@event);
            var eventHandlerMock = new Mock<IDomainEventHandler<DomainEventFake>>();
            await dbContext.AddRangeAsync(exampleCategoriesList);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(eventHandlerMock.Object);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWorkInfra.UnitOfWork>>()
            );

            await unitOfWork.Commit(CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext(true);
            var savedCategories = await assertDbContext.Categories
                .AsNoTracking()
                .ToListAsync();

            savedCategories.Should().HaveCount(exampleCategoriesList.Count);
            eventHandlerMock.Verify(x => 
                x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once
            );
            categoryWithEvent.Events.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(Rollback))]
        [Trait("Integration/Infra.Data", "UnitOfWork - Persistence")]
        public async Task Rollback()
        {
            var dbId = Guid.NewGuid().ToString();
            var dbContext = _fixture.CreateDbContext();
            var exampleCategoriesList = _fixture.GetExampleCategoriesList();
            await dbContext.AddRangeAsync(exampleCategoriesList);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWorkInfra.UnitOfWork(
                dbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWorkInfra.UnitOfWork>>()
            );

            var task = async () => await unitOfWork.Rollback(CancellationToken.None);

            await task.Should().NotThrowAsync();
        }
    }
}
