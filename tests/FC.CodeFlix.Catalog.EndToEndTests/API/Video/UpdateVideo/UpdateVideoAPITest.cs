using FC.CodeFlix.Catalog.API.APIModels.Video;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.UpdateVideo
{
    [Collection(nameof(VideoBaseFixture))]
    public class UpdateVideoAPITest : IDisposable
    {
        private readonly VideoBaseFixture _fixture;

        public UpdateVideoAPITest(VideoBaseFixture fixture)
            => _fixture = fixture;


        [Fact(DisplayName = nameof(UpdateVideo))]
        [Trait("EndToEnd/API", "Video/Update - Endpoints")]
        public async Task UpdateVideo()
        {
            var exampleVideos = _fixture.GetVideoCollection(10);
            await _fixture.VideoPersistence.InsertList(exampleVideos);

            var targetVideoId = exampleVideos.ElementAt(5).Id;

            var input = new UpdateVideoAPIInput()
            {
                Title = _fixture.GetValidTitle(),
                Description = _fixture.GetValidDescription(),
                Duration = _fixture.GetValidDuration(),
                Opened = _fixture.GetRandomBoolean(),
                Published = _fixture.GetRandomBoolean(),
                Rating = _fixture.GetRandomRating().ToStringSignal(),
                YearLaunched = _fixture.GetValidYearLaunched()
            };

            var (response, output) = await _fixture.APIClient
                .Put<TestAPIResponse<VideoModelOutput>>($"/api/videos/{targetVideoId}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();

            output.Data.Id.Should().Be(targetVideoId);
            output.Data.Title.Should().Be(input.Title);
            output.Data.Description.Should().Be(input.Description);
            output.Data.YearLaunched.Should().Be(input.YearLaunched);
            output.Data.Opened.Should().Be(input.Opened);
            output.Data.Published.Should().Be(input.Published);
            output.Data.Duration.Should().Be(input.Duration);
            output.Data.Rating.Should().Be(input.Rating);

            var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
            videoFromDb.Should().NotBeNull();
            videoFromDb.Id.Should().NotBeEmpty();
            videoFromDb.Title.Should().Be(input.Title);
            videoFromDb.Description.Should().Be(input.Description);
            videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
            videoFromDb.Opened.Should().Be(input.Opened);
            videoFromDb.Duration.Should().Be(input.Duration);
            videoFromDb.Rating.ToStringSignal().Should().Be(input.Rating);
        }

        [Fact(DisplayName = nameof(UpdateVideoWithRelationships))]
        [Trait("EndToEnd/API", "Video/Update - Endpoints")]
        public async Task UpdateVideoWithRelationships()
        {
            var exampleCategories = _fixture.GetExampleCategoriesList(3);
            var exampleGenres = _fixture.GetExampleListGenres(4);
            var exampleCastMembers = _fixture.GetExampleCastMembersList(5);
            var exampleVideos = _fixture.GetVideoCollection(10);
            exampleVideos.ForEach(video =>
            {
                exampleCategories.ForEach(category =>
                    video.AddCategory(category.Id)
                );

                exampleGenres.ForEach(genre =>
                    video.AddGenre(genre.Id)
                );

                exampleCastMembers.ForEach(castMember =>
                    video.AddCastMember(castMember.Id)
                );
            });

            await _fixture.CategoryPersistence.InsertList(exampleCategories);
            await _fixture.GenrePersistence.InsertList(exampleGenres);
            await _fixture.CastMemberPersistence.InsertList(exampleCastMembers);
            await _fixture.VideoPersistence.InsertList(exampleVideos);

            var targetVideoId = exampleVideos.ElementAt(5).Id;
            var targetCategories = new[]
            {
                exampleCategories.ElementAt(1),
            };

            var targetGenres = new[]
            {
                exampleGenres.ElementAt(0),
                exampleGenres.ElementAt(2),
            };

            var targetCastMembers = new[]
            {
                exampleCastMembers.ElementAt(1),
                exampleCastMembers.ElementAt(2),
                exampleCastMembers.ElementAt(3),
            };

            var input = new UpdateVideoAPIInput()
            {
                Title = _fixture.GetValidTitle(),
                Description = _fixture.GetValidDescription(),
                Duration = _fixture.GetValidDuration(),
                Opened = _fixture.GetRandomBoolean(),
                Published = _fixture.GetRandomBoolean(),
                Rating = _fixture.GetRandomRating().ToStringSignal(),
                YearLaunched = _fixture.GetValidYearLaunched(),
                CategoriesIds = targetCategories.Select(x => x.Id).ToList(),
                GenresIds = targetGenres.Select(x => x.Id).ToList(),
                CastMembersIds = targetCastMembers.Select(x => x.Id).ToList()
            };

            var (response, output) = await _fixture.APIClient
                .Put<TestAPIResponse<VideoModelOutput>>($"/api/videos/{targetVideoId}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();

            output.Data.Id.Should().Be(targetVideoId);
            output.Data.Title.Should().Be(input.Title);
            output.Data.Description.Should().Be(input.Description);
            output.Data.YearLaunched.Should().Be(input.YearLaunched);
            output.Data.Opened.Should().Be(input.Opened);
            output.Data.Published.Should().Be(input.Published);
            output.Data.Duration.Should().Be(input.Duration);
            output.Data.Rating.Should().Be(input.Rating);

            var expectedCategories = targetCategories
                .Select(category => new VideoModelOutputRelatedAggregate(category.Id));
            output.Data.Categories.Should().BeEquivalentTo(expectedCategories);

            var expectedGenres = targetGenres
                .Select(genre => new VideoModelOutputRelatedAggregate(genre.Id));
            output.Data.Genres.Should().BeEquivalentTo(expectedGenres);

            var expectedCastMembers = targetCastMembers
                .Select(castMember => new VideoModelOutputRelatedAggregate(castMember.Id));
            output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);

            var videoFromDb = await _fixture.VideoPersistence.GetById(output.Data.Id);
            videoFromDb.Should().NotBeNull();
            videoFromDb.Id.Should().NotBeEmpty();
            videoFromDb.Title.Should().Be(input.Title);
            videoFromDb.Description.Should().Be(input.Description);
            videoFromDb.YearLaunched.Should().Be(input.YearLaunched);
            videoFromDb.Opened.Should().Be(input.Opened);
            videoFromDb.Duration.Should().Be(input.Duration);
            videoFromDb.Rating.ToStringSignal().Should().Be(input.Rating);

            var categoriesFromDb = await _fixture.VideoPersistence
                .GetVideosCategories(targetVideoId);
            var categoriesIdsFromDb = categoriesFromDb.Select(x => x.CategoryId);
            input.CategoriesIds.Should().BeEquivalentTo(categoriesIdsFromDb);

            var genresFromDb = await _fixture.VideoPersistence
                .GetVideosGenres(targetVideoId);
            var genresIdsFromDb = genresFromDb.Select(x => x.GenreId);
            input.GenresIds.Should().BeEquivalentTo(genresIdsFromDb);

            var castMembersFromDb = await _fixture.VideoPersistence
                .GetVideosCastMembers(targetVideoId);
            var castMembersIdsFromDb = castMembersFromDb.Select(x => x.CastMemberId);
            input.CastMembersIds.Should().BeEquivalentTo(castMembersIdsFromDb);

        }


        [Fact(DisplayName = nameof(Error404WhenVideoIdNotFound))]
        [Trait("EndToEnd/API", "Video/Update - Endpoints")]
        public async Task Error404WhenVideoIdNotFound()
        {
            var exampleVideos = _fixture.GetVideoCollection(10);
            await _fixture.VideoPersistence.InsertList(exampleVideos);

            var videoId = Guid.NewGuid();

            var input = new UpdateVideoAPIInput()
            {
                Title = _fixture.GetValidTitle(),
                Description = _fixture.GetValidDescription(),
                Duration = _fixture.GetValidDuration(),
                Opened = _fixture.GetRandomBoolean(),
                Published = _fixture.GetRandomBoolean(),
                Rating = _fixture.GetRandomRating().ToStringSignal(),
                YearLaunched = _fixture.GetValidYearLaunched()
            };

            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/videos/{videoId}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Video '{videoId}' not found.");
        }

        [Fact(DisplayName = nameof(Error422WhenCategoryIdNotFound))]
        [Trait("EndToEnd/API", "Video/Update - Endpoints")]
        public async Task Error422WhenCategoryIdNotFound()
        {
            var exampleVideos = _fixture.GetVideoCollection(10);
            await _fixture.VideoPersistence.InsertList(exampleVideos);
            var categoryId = Guid.NewGuid();

            var videoId = exampleVideos.ElementAt(4).Id;

            var input = new UpdateVideoAPIInput()
            {
                Title = _fixture.GetValidTitle(),
                Description = _fixture.GetValidDescription(),
                Duration = _fixture.GetValidDuration(),
                Opened = _fixture.GetRandomBoolean(),
                Published = _fixture.GetRandomBoolean(),
                Rating = _fixture.GetRandomRating().ToStringSignal(),
                YearLaunched = _fixture.GetValidYearLaunched(),
                CategoriesIds = new List<Guid>() { categoryId }
            };

            var (response, output) = await _fixture.APIClient
                .Put<ProblemDetails>($"/api/videos/{videoId}", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableContent);
            output.Should().NotBeNull();
            output.Type.Should().Be("RelatedAggregate");
            output.Detail.Should().Be($"Related category id or ids not found: '{categoryId}'");
        }

        public void Dispose()
            => _fixture.CleanPersistence();
    }
}
