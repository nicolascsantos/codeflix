using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using UseCase = FC.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.DeleteCastMember
{
    [Collection(nameof(CastMemberUseCasesTestFixture))]
    public class DeleteCastMemberTest
    {
        private readonly CastMemberUseCasesTestFixture _fixture;

        public DeleteCastMemberTest(CastMemberUseCasesTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(DeleteCastMember))]
        [Trait("Integration/Application", "DeleteCastMember - UseCases")]
        public async Task DeleteCastMember()
        {
            var example = _fixture.GetExampleCastMember();
            var arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddAsync(example);
            await arrangeDbContext.SaveChangesAsync();
            var actDbContext = _fixture.CreateDbContext(true);
            var repository = new Repository.CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);

            var input = new UseCase.DeleteCastMemberInput(example.Id);
            var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
            var output = await useCase.Handle(input, CancellationToken.None);

            var assertDbContext = _fixture.CreateDbContext(true);
            var deletedCastMember = await actDbContext.CastMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == example.Id);
            deletedCastMember.Should().BeNull();
            var list = await actDbContext.CastMembers.AsNoTracking().ToListAsync();
            list.Should().HaveCount(0);
        }

        [Fact(DisplayName = nameof(ThrowsWhenCastMemberNotFound))]
        [Trait("Integration/Application", "DeleteCastMember - UseCases")]
        public async Task ThrowsWhenCastMemberNotFound()
        {
            var arrangeDbContext = _fixture.CreateDbContext();
            var actDbContext = _fixture.CreateDbContext(true);
            var repository = new Repository.CastMemberRepository(actDbContext);
            var unitOfWork = new UnitOfWork(actDbContext);

            var randomGuid = Guid.NewGuid();
            var input = new UseCase.DeleteCastMemberInput(randomGuid);
            var useCase = new UseCase.DeleteCastMember(repository, unitOfWork);
            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Cast member '{randomGuid}' not found.");
        }
    }
}
