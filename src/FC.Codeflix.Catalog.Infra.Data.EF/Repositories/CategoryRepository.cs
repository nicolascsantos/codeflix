using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CodeflixCatalogDbContext _context;
        private DbSet<Category> _categories => _context.Set<Category>();

        public CategoryRepository(CodeflixCatalogDbContext context)
        {
            _context = context;
        }

        public Task Delete(Category aggregate, CancellationToken cancellationToken)
        {
            return Task.FromResult(_categories.Remove(aggregate));
        }


        public async Task<Category> Get(Guid id, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
            return category ?? throw new NotFoundException($"Category '{id}' not found.");
        }


        public async Task Insert(Category aggregate, CancellationToken cancellationToken)
            => await _categories.AddAsync(aggregate, cancellationToken);

        public async Task<SearchOutput<Category>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _categories.AsNoTracking();
            query = AddOrderToQuery(query, input.OrderBy, input.Order);
            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(x => x.Name.Contains(input.Search));


            var total = await query.CountAsync();
            var items = await query.AsNoTracking()
                .Skip(toSkip)
                .Take(input.PerPage)
                .ToListAsync();
            return new SearchOutput<Category>(input.Page, input.PerPage, total, items);
        }

        public async Task Update(Category aggregate, CancellationToken cancellationToken)
            => await Task.FromResult(_context.Categories.Update(aggregate));

        private IQueryable<Category> AddOrderToQuery(IQueryable<Category> query, string orderProperty, SearchOrder order)
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

        public async Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> list, CancellationToken cancellationToken)
            => await _categories.AsNoTracking().Where(category => list.Contains(category.Id)).Select(category => category.Id).ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Category>> GetListByIds(List<Guid> list, CancellationToken cancellationToken)
            => await _categories
                        .AsNoTracking()
                        .Where(category => list.Contains(category.Id))
                        .ToListAsync(cancellationToken);
    }
}
