using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.GetCastMember
{
    [Collection(nameof(CastMemberUseCasesTestFixture))]
    public class GetCastMemberTest
    {
        private readonly CastMemberUseCasesTestFixture _fixture;

        public GetCastMemberTest(CastMemberUseCasesTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetCastMember))]
        [Trait("Integration/Application", "GetCastMember - UseCases")]
        public async Task GetCastMember()
        {
            var assertDbContext = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(assertDbContext);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                assertDbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<IUnitOfWork>>()
            );
            var castMembersExampleList = _fixture.GetCastMembersListExample(10);
            var example = castMembersExampleList[5];
            await assertDbContext.AddRangeAsync(castMembersExampleList);
            await assertDbContext.SaveChangesAsync();

            var input = new UseCase.GetCastMemberInput(example.Id);
            var useCase = new UseCase.GetCastMember(repository);
            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(example.Name);
            output.Type.Should().Be(example.Type);
            output.Id.Should().NotBeEmpty();
        }

        [Fact(DisplayName = nameof(ThrowsWhenCastMemberNotFound))]
        [Trait("Integration/Application", "GetCastMember - UseCases")]
        public async Task ThrowsWhenCastMemberNotFound()
        {
            var assertDbContext = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(assertDbContext);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var eventPublisher = new DomainEventPublisher(serviceProvider);
            var unitOfWork = new UnitOfWork(
                assertDbContext,
                eventPublisher,
                serviceProvider.GetRequiredService<ILogger<UnitOfWork>>()
            );
            var randomGuid = Guid.NewGuid();

            var input = new UseCase.GetCastMemberInput(randomGuid);
            var useCase = new UseCase.GetCastMember(repository);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Cast member '{randomGuid}' not found.");
        }
    }
}
