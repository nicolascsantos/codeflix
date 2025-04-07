using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        public Task Delete(Genre aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Genre> Get(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
