using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
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

        [Fact(DisplayName = nameof(Search))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task Search()
        {

            var arrangeDbContext = _fixture.CreateDbContext();
            var castMemberExampleList = _fixture.GetCastMembersListExample(10);
            await arrangeDbContext.AddRangeAsync(castMemberExampleList);
            await arrangeDbContext.SaveChangesAsync();
            var repository = new Repository.CastMemberRepository(arrangeDbContext);

            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);

            var search = await repository.Search(searchInput, CancellationToken.None);

            search.Should().NotBeNull();
            search.CurrentPage.Should().Be(searchInput.Page);
            search.PerPage.Should().Be(searchInput.PerPage);
            search.Total.Should().Be(castMemberExampleList.Count);
            search.Items.Should().HaveCount(castMemberExampleList.Count);
            search.Items.ToList().ForEach(item =>
            {
                var castMemberExample = castMemberExampleList.Find(x => x.Id == item.Id);
                castMemberExample.Should().NotBeNull();
                castMemberExample.Name.Should().Be(item.Name);
                castMemberExample.Type.Should().Be(item.Type);
            });
        }

        [Fact(DisplayName = nameof(SearchReturnsEmpty))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task SearchReturnsEmpty()
        {
            var arrangeDbContext = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(arrangeDbContext);

            var searchInput = new SearchInput(1, 20, "", "", SearchOrder.Asc);
            var search = await repository.Search(searchInput, CancellationToken.None);

            search.Should().NotBeNull();
            search.CurrentPage.Should().Be(1);
            search.PerPage.Should().Be(20);
            search.Total.Should().Be(0);
            search.Items.Should().HaveCount(0);
        }

        [Theory(DisplayName = nameof(SearchReturnsPaginated))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task SearchReturnsPaginated(
            int amountOfCastMembersToGenerate,
            int page,
            int perPage,
            int expectedQuantityItems)
        {
            var context = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(context);
            var castMembersExampleList = _fixture.GetCastMembersListExample(amountOfCastMembersToGenerate);
            await context.AddRangeAsync(castMembersExampleList);
            await context.SaveChangesAsync();

            var input = new SearchInput(page, perPage, "", "", SearchOrder.Asc);

            var output = await repository.Search(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.CurrentPage.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Total.Should().Be(amountOfCastMembersToGenerate);
            output.Items.Should().HaveCount(expectedQuantityItems);
            output.Items.ToList().ForEach(item =>
            {
                var exampleCastMember = castMembersExampleList.Find(x => x.Id == item.Id);
                exampleCastMember.Should().NotBeNull();
                exampleCastMember.Name.Should().Be(item.Name);
                exampleCastMember.Type.Should().Be(item.Type);
            });
        }

        [Theory(DisplayName = nameof(SearchByText))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        [InlineData("Action", 1, 5, 1, 1)]
        [InlineData("Horror", 1, 5, 3, 3)]
        [InlineData("Horror", 2, 5, 0, 3)]
        [InlineData("Sci-fi", 1, 5, 4, 4)]
        [InlineData("Sci-fi", 1, 2, 2, 4)]
        [InlineData("Sci-fi", 2, 3, 1, 4)]
        [InlineData("Sci-fi Other", 1, 3, 0, 0)]
        [InlineData("Robots", 1, 5, 2, 2)]
        public async Task SearchByText(
            string search,
            int page,
            int perPage,
            int expectedQuantityItemsReturned,
            int expectedQuantityTotalItems
        )
        {
            var dbContext = _fixture.CreateDbContext();
            var castMemberExample = _fixture.GetExampleCastMember();
            var exampleCastMembersList = _fixture.GetExampleCastMembersListByNames(new List<string>()
            {
                "Action",
                "Horror",
                "Horror - Robots",
                "Horror - Based on Real Facts",
                "Drama",
                "Sci-fi AI",
                "Sci-fi Space",
                "Sci-fi Robots",
                "Sci-fi Future"
            });
            exampleCastMembersList.Add(castMemberExample);
            await dbContext.AddRangeAsync(exampleCastMembersList);
            await dbContext.SaveChangesAsync();
            var castMemberRepository = new Repository.CastMemberRepository(dbContext);

            var searchInput = new SearchInput(
                page: page,
                perPage: perPage,
                search: search,
                orderBy: "",
                searchOrder: SearchOrder.Asc
            );

            var output = await castMemberRepository.Search(searchInput, CancellationToken.None);

            output.Should().NotBeNull();
            output.CurrentPage.Should().Be(searchInput.Page);
            output.PerPage.Should().Be(searchInput.PerPage);
            output.Total.Should().Be(expectedQuantityTotalItems);
            output.Items.Should().HaveCount(expectedQuantityItemsReturned);
            foreach (CastMember outputItem in output.Items)
            {
                var exampleItem = exampleCastMembersList.Find(i => i.Id == outputItem.Id);
                exampleItem.Should().NotBeNull();
                outputItem.Name.Should().Be(exampleItem.Name);
                outputItem.Type.Should().Be(exampleItem.Type);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
            }
        }
    }
}
