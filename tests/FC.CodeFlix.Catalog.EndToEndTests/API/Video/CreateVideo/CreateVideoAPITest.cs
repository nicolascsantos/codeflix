using FC.CodeFlix.Catalog.API.APIModels.Response;
using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.CreateVideo
{
    [Collection(nameof(VideoBaseFixture))]
    public class CreateVideoAPITest : IDisposable
    {
        private readonly VideoBaseFixture _fixture;

        public CreateVideoAPITest(VideoBaseFixture fixture)
            => _fixture = fixture;

        public void Dispose()
            => _fixture.CleanPersistence();

        [Fact(DisplayName = nameof(CreateBasicVideo))]
        [Trait("EndToEnd/API", "Video/Create - Endpoints")]
        public async Task CreateBasicVideo()
        {
            CreateVideoAPIInput input = _fixture.GetBasicCreateVideoInput();

            var (response, output) = await 
                _fixture.APIClient.Post<APIResponse<VideoModelOutput>>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Data.Id.Should().NotBeEmpty();
            output.Data.Title.Should().Be(input.Title);
            output.Data.Description.Should().Be(input.Description);
            output.Data.YearLaunched.Should().Be(input.YearLaunched);
            output.Data.Opened.Should().Be(input.Opened);
            output.Data.Duration.Should().Be(input.Duration);
            output.Data.Rating.Should().Be(input.Rating.ToStringSignal());

            var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
            videoFromDb.Should().NotBeNull();
            videoFromDb.Id.Should().NotBeEmpty();
            videoFromDb.Title.Should().Be(input.Title);
            videoFromDb.Description.Should().Be(input.Description);
            videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
            videoFromDb.Opened.Should().Be(input.Opened);
            videoFromDb.Duration.Should().Be(input.Duration);
            videoFromDb.Rating.Should().Be(input.Rating);
        }

        [Fact(DisplayName = nameof(CreateVideoWithRelationships))]
        [Trait("EndToEnd/API", "Video/Create - Endpoints")]
        public async Task CreateVideoWithRelationships()
        {
            var categories = _fixture.GetExampleCategoriesList();
            await _fixture.CategoryPersistence.InsertList(categories);

            var genres = _fixture.GetExampleListGenres();
            await _fixture.GenrePersistence.InsertList(genres);

            var castMembers = _fixture.GetExampleCastMembersList();
            await _fixture.CastMemberPersistence.InsertList(castMembers);

            CreateVideoAPIInput input = _fixture.GetBasicCreateVideoInput();
            input.CategoriesIds = categories.Select(x => x.Id).ToList();
            input.GenresIds = genres.Select(x => x.Id).ToList();
            input.CastMembersIds = castMembers.Select(x => x.Id).ToList();

            var (response, output) = await
                _fixture.APIClient.Post<APIResponse<VideoModelOutput>>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status201Created);
            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Data.Id.Should().NotBeEmpty();
            output.Data.Title.Should().Be(input.Title);
            output.Data.Description.Should().Be(input.Description);
            output.Data.YearLaunched.Should().Be(input.YearLaunched);
            output.Data.Opened.Should().Be(input.Opened);
            output.Data.Duration.Should().Be(input.Duration);
            output.Data.Rating.Should().Be(input.Rating.ToStringSignal());

            var outputCategoriesIds = output.Data.Categories.Select(x => x.Id).ToList();
            outputCategoriesIds.Should().NotBeEmpty();
            outputCategoriesIds.Should().BeEquivalentTo(input.CategoriesIds);

            var outputGenresIds = output.Data.Genres.Select(x => x.Id).ToList();
            outputGenresIds.Should().NotBeEmpty();
            outputGenresIds.Should().BeEquivalentTo(input.GenresIds);

            var outputCastMembersIds = output.Data.CastMembers.Select(x => x.Id).ToList();
            outputCastMembersIds.Should().NotBeEmpty();
            outputCastMembersIds.Should().BeEquivalentTo(input.CastMembersIds);

            var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
            videoFromDb.Should().NotBeNull();
            videoFromDb.Id.Should().NotBeEmpty();
            videoFromDb.Title.Should().Be(input.Title);
            videoFromDb.Description.Should().Be(input.Description);
            videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
            videoFromDb.Opened.Should().Be(input.Opened);
            videoFromDb.Duration.Should().Be(input.Duration);
            videoFromDb.Rating.Should().Be(input.Rating);
            var categoriesFromDb = await _fixture.VideoPersistence
                .GetVideosCategories(videoFromDb.Id);
            categories.Should().NotBeNull();
            var categoriesIdsFromDb = categoriesFromDb.Select(x => x.CategoryId).ToList();
            categoriesIdsFromDb.Should().BeEquivalentTo(input.CategoriesIds);

            var genresFromDb = await _fixture.VideoPersistence
                .GetVideosGenres(videoFromDb.Id);
            genresFromDb.Should().NotBeNull();
            var genresIdsFromDb = genresFromDb.Select(x => x.GenreId).ToList();
            genresIdsFromDb.Should().BeEquivalentTo(input.GenresIds);

            var castMembersFromDb = await _fixture.VideoPersistence
                .GetVideosCastMembers(videoFromDb.Id);
            castMembersFromDb.Should().NotBeNull();
            var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId).ToList();
            castMembersIdsFromDb.Should().BeEquivalentTo(input.CastMembersIds);
        }

        [Fact(DisplayName = nameof(CreateVideoWithInvalidGenreId))]
        [Trait("EndToEnd/API", "Video/Create - Endpoints")]
        public async Task CreateVideoWithInvalidGenreId()
        {
            var invalidGenreId = Guid.NewGuid();
            CreateVideoAPIInput input = _fixture.GetBasicCreateVideoInput();
            input.GenresIds = new List<Guid> { invalidGenreId };
            var (response, output) = await
                _fixture.APIClient.Post<ProblemDetails>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related genre id or ids not found: '{invalidGenreId}'");
        }

        [Fact(DisplayName = nameof(CreateVideoWithInvalidCategoryId))]
        [Trait("EndToEnd/API", "Video/Create - Endpoints")]
        public async Task CreateVideoWithInvalidCategoryId()
        {
            var invalidCategoryId = Guid.NewGuid();
            CreateVideoAPIInput input = _fixture.GetBasicCreateVideoInput();
            input.CategoriesIds = new List<Guid> { invalidCategoryId };
            var (response, output) = await
                _fixture.APIClient.Post<ProblemDetails>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related category id or ids not found: '{invalidCategoryId}'");
        }

        [Fact(DisplayName = nameof(CreateVideoWithInvalidCastMemberId))]
        [Trait("EndToEnd/API", "Video/Create - Endpoints")]
        public async Task CreateVideoWithInvalidCastMemberId()
        {
            var invalidCastMemberId = Guid.NewGuid();
            CreateVideoAPIInput input = _fixture.GetBasicCreateVideoInput();
            input.CastMembersIds = new List<Guid> { invalidCastMemberId };
            var (response, output) = await
                _fixture.APIClient.Post<ProblemDetails>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status422UnprocessableEntity);
            output.Should().NotBeNull();
            output.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related cast member id or ids not found: '{invalidCastMemberId}'");
        }
    }
}
