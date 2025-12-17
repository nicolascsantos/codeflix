using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly CodeflixCatalogDbContext _context;

        public VideoRepository(CodeflixCatalogDbContext context)
            => _context = context;

        private DbSet<Video> _videos => _context.Set<Video>();


        public Task Delete(Video aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Video> Get(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Video>> GetListByIds(List<Guid> list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(Video aggregate, CancellationToken cancellationToken)
        {
            if (aggregate.Categories.Count > 0)
            {

            }

            await _videos.AddAsync(aggregate, cancellationToken);
        }

        public Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Update(Video aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
