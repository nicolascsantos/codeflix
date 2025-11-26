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
        

        public  Task Delete(CastMember aggregate, CancellationToken cancellationToken)
            => Task.FromResult(_castMembers.Remove(aggregate));

        public async Task<CastMember> Get(Guid id, CancellationToken cancellationToken)
            => await _castMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException($"Cast member '{id}' not found.");

        public async Task Insert(CastMember aggregate, CancellationToken cancellationToken)
            => await _castMembers.AddAsync(aggregate, cancellationToken);

        public async Task<SearchOutput<CastMember>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _castMembers.AsNoTracking();
            query = AddOrderToQuery(query, input.OrderBy, input.Order);
            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(x => x.Name.Contains(input.Search));


            var total = await query.CountAsync();
            var items = await query.AsNoTracking()
                .Skip(toSkip)
                .Take(input.PerPage)
                .ToListAsync();
            return new SearchOutput<CastMember>(input.Page, input.PerPage, total, items);
        }

        public Task Update(CastMember aggregate, CancellationToken cancellationToken)
            => Task.FromResult(_castMembers.Update(aggregate));

         private IQueryable<CastMember> AddOrderToQuery(IQueryable<CastMember> query, string orderProperty, SearchOrder order)
        => (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => query.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => query.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
        };

        public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CastMember>> GetListByIds(List<Guid> list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
