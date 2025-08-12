using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Genre.ListGenre
{
    [CollectionDefinition(nameof(ListGenreTestFixture))]
    public class ListGenreTestFixtureCollection : ICollectionFixture<ListGenreTestFixture> { }

    public class ListGenreTestFixture : GenreUseCaseBaseFixture
    {
        public List<DomainEntity.Genre> CloneGenreListOrdered(List<DomainEntity.Genre> genresList, string orderBy, SearchOrder order)
        {
            var listClone = new List<DomainEntity.Genre>(genresList);
            var orderedEnumerable = (orderBy.ToLower(), order) switch
            {
                ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
                ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
                ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
                ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
                ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
                ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
                _ => listClone.OrderBy(x => x.Name)
            };
            return orderedEnumerable.ToList();
        }
    }
}
