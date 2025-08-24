using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Genre.Common
{
    public class GenrePersistence
    {
        private readonly CodeflixCatalogDbContext _context;

        public GenrePersistence(CodeflixCatalogDbContext context)
            => _context = context;

        public async Task InsertList(List<DomainEntity.Genre> genres)
        {

            await _context.Genres.AddRangeAsync(genres);
            var test = await _context.SaveChangesAsync();
        }

        public async Task InsertGenresCategoriesRelationsList(List<GenresCategories> genresCategories)
        {
            await _context.GenresCategories.AddRangeAsync(genresCategories);
            await _context.SaveChangesAsync();
        }

        public async Task<DomainEntity.Genre?> GetById(Guid id)
            => await _context.Genres.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<GenresCategories>> GetGenresCategoriesRelationsByGenreId(Guid id)
            => await _context.GenresCategories
                .AsNoTracking()
                .Where(x => x.GenreId == id)
                .ToListAsync();
    }
}
