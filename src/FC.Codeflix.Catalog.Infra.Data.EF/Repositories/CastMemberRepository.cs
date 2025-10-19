using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class CastMemberRepository : ICastMemberRepository
    {
        private readonly CodeflixCatalogDbContext _context;

        private DbSet<CastMember> _castMembers => _context.Set<CastMember>();

        public CastMemberRepository(CodeflixCatalogDbContext context)
            => _context = context;
        

        public Task Delete(CastMember aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
            => await _castMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException($"Cast member '{id}' not found.");

        public async Task Insert(CastMember aggregate, CancellationToken cancellationToken)
            => await _castMembers.AddAsync(aggregate, cancellationToken);

        public Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Update(CastMember aggregate, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
