using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.UpdateCastMember
{
    [Collection(nameof(CastMemberUseCasesTestFixture))]
    public class UpdateCastMemberTest
    {
        private CastMemberUseCasesTestFixture _fixture;

        public UpdateCastMemberTest(CastMemberUseCasesTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(UpdateCastMember))]
        [Trait("Integration/Application", "UpdateCastMember - UseCases")]
        public async Task UpdateCastMember()
        {
            var examples = _fixture.GetCastMembersListExample(10);
            var example = examples[5];
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(examples);
            await arrangeDbContext.SaveChangesAsync();
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            var actDbContext = _fixture.CreateDbContext(true);
            var repository = new Repository.CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);
            var useCase = new UseCase.UpdateCastMember(repository, unitOfWork);
            var input = new UseCase.UpdateCastMemberInput(example.Id, newName, newType);

            var output = await useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Id.Should().Be(example.Id);
            output.Name.Should().Be(newName);
            output.Type.Should().Be(newType);
            var item = await _fixture.CreateDbContext(true).CastMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == example.Id);
            item.Should().NotBeNull();
            item.Name.Should().Be(newName);
            item.Type.Should().Be(newType);
        }

        [Fact(DisplayName = nameof(ThrowsWhenCastMemberNotFound))]
        [Trait("Integration/Application", "UpdateCastMember - UseCases")]
        public async Task ThrowsWhenCastMemberNotFound()
        {
            var randomGuid = Guid.NewGuid();
            var newName = _fixture.GetValidName();
            var newType = _fixture.GetRandomCastMemberType();
            var actDbContext = _fixture.CreateDbContext(true);
            var repository = new Repository.CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);
            var useCase = new UseCase.UpdateCastMember(repository, unitOfWork);
            var input = new UseCase.UpdateCastMemberInput(randomGuid, newName, newType);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Cast member '{randomGuid}' not found.");
        }
    }
}
