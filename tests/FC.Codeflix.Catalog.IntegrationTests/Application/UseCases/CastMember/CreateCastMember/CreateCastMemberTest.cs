using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.CastMember.CreateCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember
{
    [Collection(nameof(CreateCastMemberTestFixture))]
    public class CreateCastMemberTest
    {
        private readonly CreateCastMemberTestFixture _fixture;

        public CreateCastMemberTest(CreateCastMemberTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateCastMember))]
        [Trait("Integration/Application", "CreateCastMember - UseCases")]
        public async Task CreateCastMember()
        {
            var validCastMember = _fixture.GetExampleCastMember();
            var actDbContext = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);

            var useCase = new UseCase.CreateCastMember(repository, unitOfWork);
            var input = new CreateCastMemberInput(validCastMember.Name, validCastMember.Type);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Name.Should().Be(input.Name);
            output.Type.Should().Be(input.Type);
            output.Id.Should().NotBeEmpty();
            output.CreatedAt.Should().NotBe(default);

            var assertDbContext = _fixture.CreateDbContext(true);
            var castMembers = await assertDbContext.CastMembers
                .AsNoTracking()
                .ToListAsync();
            castMembers.Should().HaveCount(1);
            var castMemberFromDb = castMembers[0];
            castMemberFromDb.Name.Should().Be(input.Name);
            castMemberFromDb.Type.Should().Be(input.Type);
            castMemberFromDb.Id.Should().Be(output.Id);
        }
    }
}
