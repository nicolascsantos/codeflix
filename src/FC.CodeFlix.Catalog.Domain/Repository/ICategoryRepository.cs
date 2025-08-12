using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Domain.Repository
{
    public interface ICategoryRepository : IGenericRepository<Category>, ISearchableRepository<Category>
    {
        public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> list, CancellationToken cancellationToken);

        public Task<IReadOnlyList<Category>> GetListByIds(List<Guid> list, CancellationToken cancellationToken);
    }
}
