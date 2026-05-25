using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common
{
    public class VideoPersistence
    {
        private readonly CodeflixCatalogDbContext _context;

        public VideoPersistence(CodeflixCatalogDbContext context)
            => _context = context;
        

        public async Task InsertList(List<DomainEntity.Video> videos)
        {
            await _context.Videos.AddRangeAsync(videos);
            await _context.SaveChangesAsync();
        }

        public async Task<DomainEntity.Video?> GetById(Guid id)
            => await _context.Videos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<VideosCastMembers>> GetVideosCastMembers(Guid videoId)
            => await _context.VideosCastMembers
                .AsNoTracking()
                .Where(x => x.VideoId == videoId)
                .ToListAsync();

        public async Task<List<VideosGenres>> GetVideosGenres(Guid videoId)
            => await _context.VideosGenres
                .AsNoTracking()
                .Where(x => x.VideoId == videoId)
                .ToListAsync();

        public async Task<List<VideosCategories>> GetVideosCategories(Guid videoId)
            => await _context.VideosCategories
                .AsNoTracking()
                .Where(x => x.VideoId == videoId)
                .ToListAsync();
    }
}
