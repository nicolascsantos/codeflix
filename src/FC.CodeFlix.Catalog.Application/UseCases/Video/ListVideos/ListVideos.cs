
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Repository;
using DomainEntities = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos
{
    public class ListVideos : IListVideos
    {
        private readonly IVideoRepository _videoRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ICastMemberRepository _castMemberRepository;

        public ListVideos(
            IVideoRepository videoRepository,
            ICategoryRepository categoryRepository,
            IGenreRepository genreRepository,
            ICastMemberRepository castMemberRepository
        )
        {
            _videoRepository = videoRepository;
            _categoryRepository = categoryRepository;
            _genreRepository = genreRepository;
            _castMemberRepository = castMemberRepository;
        }

        public async Task<ListVideosOutput> Handle(ListVideosInput request, CancellationToken cancellationToken)
        {
            IReadOnlyList<DomainEntities.Category>? categories = null;
            IReadOnlyList<DomainEntities.Genre>? genres = null;
            IReadOnlyList<DomainEntities.CastMember>? castMembers = null;

            var searchResult = await _videoRepository
                .Search(request.ToSearchInput(), cancellationToken);
            var relatedCategoriesIds = searchResult.Items
                .SelectMany(video => video.Categories)
                .Distinct()
                .ToList();

            var relatedGenresIds = searchResult.Items
                .SelectMany(video => video.Genres)
                .Distinct()
                .ToList();

            var relatedCastMembersIds = searchResult.Items
                .SelectMany(video => video.CastMembers)
                .Distinct()
                .ToList();

            if (relatedCategoriesIds is not null && relatedCategoriesIds.Count > 0)
            {
                categories = await _categoryRepository.GetListByIds(relatedCategoriesIds, cancellationToken);
            }

            if (relatedGenresIds is not null && relatedGenresIds.Count > 0)
            {
                genres = await _genreRepository.GetListByIds(relatedGenresIds, cancellationToken);
            }

            if (relatedCastMembersIds is not null && relatedCastMembersIds.Count > 0)
            {
                castMembers = await _castMemberRepository.GetListByIds(relatedCastMembersIds, cancellationToken);
            }

            var output =  new ListVideosOutput(
                searchResult.CurrentPage,
                searchResult.PerPage,
                searchResult.Total,
                searchResult.Items.Select(item => VideoModelOutput.FromVideo(item, categories, genres, castMembers)).ToList()
            );

            return output;
        }
    }
}
