using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.Common
{
    public class CategoryPersistence
    {
        private readonly CodeflixCatalogDbContext _context;

        public CategoryPersistence(CodeflixCatalogDbContext context)
            => _context = context;

        public async Task<DomainEntity.Category?> GetById(Guid id)
            => await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task InsertList(List<DomainEntity.Category> categories)
        {

            await _context.Categories.AddRangeAsync(categories);
            var test = await _context.SaveChangesAsync();
        }

    }
}
