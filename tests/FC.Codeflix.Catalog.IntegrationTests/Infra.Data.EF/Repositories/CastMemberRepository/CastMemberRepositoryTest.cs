using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository
{
    [Collection(nameof(CastMemberRepositoryTestFixture))]
    public class CastMemberRepositoryTest
    {
        private readonly CastMemberRepositoryTestFixture _fixture;

        public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Insert))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task Insert()  
        {
            var castMemberExample = _fixture.GetExampleCastMember();
            CodeflixCatalogDbContext context = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(context);

            await repository.Insert(castMemberExample, CancellationToken.None);
            await context.SaveChangesAsync();

            var assertionContext = _fixture.CreateDbContext(true);
            var castMemberFromDb = await assertionContext.CastMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);

            castMemberFromDb.Should().NotBeNull();
            castMemberFromDb.Name.Should().Be(castMemberExample.Name);
            castMemberFromDb.Type.Should().Be(castMemberExample.Type);
        }

        [Fact(DisplayName = nameof(Get))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task Get()
        {
            var castMemberExampleList = _fixture.GetCastMembersListExample(5);
            var castMemberExample = castMemberExampleList[3];
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(castMemberExampleList);
            await arrangeDbContext.SaveChangesAsync();

            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var castMemberFromRepository = await repository.Get(castMemberExample.Id, CancellationToken.None);

            castMemberFromRepository.Should().NotBeNull();
            castMemberFromRepository.Name.Should().Be(castMemberExample.Name);
            castMemberFromRepository.Type.Should().Be(castMemberExample.Type);
        }

        [Fact(DisplayName = nameof(GetThrowsWhenNotFound))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task GetThrowsWhenNotFound()
        {
            var castMemberExampleList = _fixture.GetCastMembersListExample(5);
            var randomGuid = Guid.NewGuid();
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(castMemberExampleList);
            await arrangeDbContext.SaveChangesAsync();

            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            var action = async () => await repository.Get(randomGuid, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Cast member '{randomGuid}' not found.");
        }

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task Delete()
        {
            var castMemberExampleList = _fixture.GetCastMembersListExample(5);
            var castMemberExample = castMemberExampleList[3];
            CodeflixCatalogDbContext arrangeDbContext = _fixture.CreateDbContext();
            await arrangeDbContext.AddRangeAsync(castMemberExampleList);
            await arrangeDbContext.SaveChangesAsync();

            var repository = new Repository.CastMemberRepository(_fixture.CreateDbContext(true));
            await repository.Delete(castMemberExample, CancellationToken.None);
            await arrangeDbContext.SaveChangesAsync();

            var deletedCastMember = await (_fixture.CreateDbContext()).CastMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);
            deletedCastMember.Should().BeNull();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task Update()
        {
            var castMemberExample = _fixture.GetExampleCastMember();
            var newValues = _fixture.GetExampleCastMember();
            var arrangeDbContext = _fixture.CreateDbContext();

            await arrangeDbContext.AddAsync(castMemberExample);
            await arrangeDbContext.SaveChangesAsync();

            castMemberExample.Update(newValues.Name, newValues.Type);

            var repository = new Repository.CastMemberRepository(arrangeDbContext);

            await repository.Update(castMemberExample, CancellationToken.None);
            await arrangeDbContext.SaveChangesAsync();

            var updatedCastMember = await (_fixture.CreateDbContext(true)).CastMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);

            updatedCastMember.Should().NotBeNull();
            updatedCastMember.Name.Should().Be(newValues.Name);
            updatedCastMember.Type.Should().Be(newValues.Type);
        }
    }
}
