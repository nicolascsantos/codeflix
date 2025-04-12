using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly CodeflixCatalogDbContext _context;

        private DbSet<Genre> _genres => _context.Set<Genre>();

        public GenreRepository(CodeflixCatalogDbContext context)
        {
            _context = context;
        }

        public Task Delete(Genre aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(_genres.Remove(aggregate));
        }

        public async Task<Genre> Get(Guid id, CancellationToken cancellationToken)
        {
            var genre = await _genres
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return genre ?? throw new NotFoundException($"Genre '{id}' not found.");
        }

        public Task Insert(Genre aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Update(Genre aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
