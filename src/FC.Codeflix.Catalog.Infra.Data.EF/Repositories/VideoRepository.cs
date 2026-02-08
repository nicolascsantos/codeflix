using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.Application.Exceptions;
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
        private DbSet<Media> _medias => _context.Set<Media>();
        private DbSet<VideosCategories> _videosCategories => _context.Set<VideosCategories>();
        private DbSet<VideosGenres> _videosGenres => _context.Set<VideosGenres>();
        private DbSet<VideosCastMembers> _videosCastMembers => _context.Set<VideosCastMembers>();


        public Task Delete(Video aggregate, CancellationToken cancellationToken)
        {
            _videosCategories.RemoveRange(_videosCategories.Where(x => x.VideoId == aggregate.Id));
            _videosGenres.RemoveRange(_videosGenres.Where(x => x.VideoId == aggregate.Id));
            _videosCastMembers.RemoveRange(_videosCastMembers.Where(x => x.VideoId == aggregate.Id));

            if (aggregate.Trailer is not null) _medias.Remove(aggregate.Trailer);
            if (aggregate.Media is not null) _medias.Remove(aggregate.Media);

            _videos.Remove(aggregate);
            return Task.CompletedTask;
        }

        public async Task<Video> Get(Guid id, CancellationToken cancellationToken)
        {
            var video = await _videos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id) ?? throw new NotFoundException($"Video '{id}' not found.");

            var categoriesIds = await _videosCategories.Where(x => x.VideoId == id)
                .Select(x => x.CategoryId)
                .ToListAsync();
            categoriesIds.ForEach(video.AddCategory);

            var genresIds = await _videosGenres.Where(genre => genre.VideoId == id)
                .Select(x => x.GenreId)
                .ToListAsync();
            genresIds.ForEach(video.AddGenre);

            var castMembersIds = await _videosCastMembers.Where(castMember => castMember.VideoId == id)
                .Select(x => x.CastMemberId)
                .ToListAsync();
            castMembersIds.ForEach(video.AddCastMember);

            return video;
        }

        public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Video>> GetListByIds(List<Guid> list, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Insert(Video video, CancellationToken cancellationToken)
        {


            if (video.Categories.Count > 0)
            {
                var relations = video.Categories
                    .Select(categoryId => new VideosCategories(
                        video.Id,
                        categoryId
                    ));
                await _videosCategories.AddRangeAsync(relations);
            }

            if (video.Genres.Count > 0)
            {
                var relations = video.Genres
                    .Select(genreId => new VideosGenres(
                        video.Id,
                        genreId
                    ));
                await _videosGenres.AddRangeAsync(relations);
            }

            if (video.CastMembers.Count > 0)
            {
                var relations = video.CastMembers
                    .Select(castMemberId => new VideosCastMembers(
                        video.Id,
                        castMemberId
                    ));
                await _videosCastMembers.AddRangeAsync(relations);
            }

            await _videos.AddAsync(video, cancellationToken);
        }

        public async Task<SearchOutput<Video>> Search(SearchInput input, CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;

            var query = _videos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(x => x.Title.Contains(input.Search));

            query = input switch
            {
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "title"
                    => query.OrderBy(video => video.Title).ThenBy(video => video.Id),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "title"
                    => query.OrderByDescending(video => video.Title).ThenByDescending(video => video.Id),
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "id"
                    => query.OrderBy(video => video.Id),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "id"
                    => query.OrderByDescending(video => video.Id),
                { Order: SearchOrder.Asc } when input.OrderBy.ToLower() is "createdat"
                    => query.OrderBy(video => video.CreatedAt),
                { Order: SearchOrder.Desc } when input.OrderBy.ToLower() is "createdat"
                    => query.OrderByDescending(video => video.CreatedAt),
                _ => query = query.OrderBy(video => video.Title).ThenBy(video => video.Id)
            };

            var count = query.Count();
            var items = await query.Skip(toSkip)
                .Take(input.PerPage)
                .ToListAsync(cancellationToken);

            return new SearchOutput<Video>(
                input.Page,
                input.PerPage,
                count,
                items
            );
        }

        public async Task Update(Video video, CancellationToken cancellationToken)
        {
            _videos.Update(video);

            _videosCategories.RemoveRange(_videosCategories
                    .Where(x => x.VideoId == video.Id));
            _videosCastMembers.RemoveRange(_videosCastMembers
                .Where(x => x.VideoId == video.Id));
            _videosGenres.RemoveRange(_videosGenres
                .Where(x => x.VideoId == video.Id));

            if (video.Categories.Count > 0)
            {
                var relations = video.Categories
                    .Select(categoryId => new VideosCategories(
                        video.Id,
                        categoryId
                    ));
                await _videosCategories.AddRangeAsync(relations);
            }

            if (video.Genres.Count > 0)
            {
                var relations = video.Genres
                    .Select(genreId => new VideosGenres(
                        video.Id,
                        genreId
                    ));
                await _videosGenres.AddRangeAsync(relations);
            }

            if (video.CastMembers.Count > 0)
            {
                var relations = video.CastMembers
                    .Select(castMemberId => new VideosCastMembers(
                        video.Id,
                        castMemberId
                    ));
                await _videosCastMembers.AddRangeAsync(relations);
            }
        }
    }
}
