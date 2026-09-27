using FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.DeleteVideo
{
    [Collection(nameof(VideoBaseFixture))]
    public class DeleteVideoAPITest : IDisposable
    {
        private readonly VideoBaseFixture _fixture;

        public DeleteVideoAPITest(VideoBaseFixture fixture)
            => _fixture = fixture;


        [Fact(DisplayName = nameof(DeleteVideo))]
        [Trait("EndToEnd/API", "Video/Delete - Endpoints")]
        public async Task DeleteVideo()
        {
            var exampleVideos = _fixture.GetVideoCollection(10);
            await _fixture.VideoPersistence.InsertList(exampleVideos);
            var mediasCount = await _fixture.VideoPersistence.GetMediaCount();
            var expectedMediaCount = mediasCount - 2;

            var targetVideoId = exampleVideos.ElementAt(5).Id;

            var (response, output) = await _fixture.APIClient
                .Delete<object>($"/api/videos/{targetVideoId}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            output.Should().BeNull();

            var videoFromDb = await _fixture.VideoPersistence.GetById(targetVideoId);
            videoFromDb.Should().BeNull();
            var actualMediaCount = await _fixture.VideoPersistence.GetMediaCount();
            actualMediaCount.Should().Be(expectedMediaCount);
        }

        [Fact(DisplayName = nameof(DeleteVideoWithRelationships))]
        [Trait("EndToEnd/API", "Video/Delete - Endpoints")]
        public async Task DeleteVideoWithRelationships()
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
            var mediasCount = await _fixture.VideoPersistence.GetMediaCount();
            var expectedMediaCount = mediasCount - 2;

            var targetVideoId = exampleVideos.ElementAt(7).Id;

            var (response, output) = await _fixture.APIClient
                .Delete<object>($"/api/videos/{targetVideoId}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            output.Should().BeNull();

            var videoFromDb = await _fixture.VideoPersistence.GetById(targetVideoId);
            videoFromDb.Should().BeNull();

            var categoriesFromDb = await _fixture.VideoPersistence
                .GetVideosCategories(targetVideoId);
            categoriesFromDb.Should().BeEmpty();

            var genresFromDb = await _fixture.VideoPersistence
                .GetVideosGenres(targetVideoId);
            genresFromDb.Should().BeEmpty();

            var castMembersFromDb = await _fixture.VideoPersistence
                .GetVideosCastMembers(targetVideoId);
            castMembersFromDb.Should().BeEmpty();

            var actualMediaCount = await _fixture.VideoPersistence.GetMediaCount();
            actualMediaCount.Should().Be(expectedMediaCount);
        }

        [Fact(DisplayName = nameof(Error404WhenVideoidNotFound))]
        [Trait("EndToEnd/API", "Video/Delete - Endpoints")]
        public async Task Error404WhenVideoidNotFound()
        {
            var exampleVideos = _fixture.GetVideoCollection(10);
            await _fixture.VideoPersistence.InsertList(exampleVideos);

            var videoId = Guid.NewGuid();

            var (response, output) = await _fixture.APIClient
                .Delete<ProblemDetails>($"/api/videos/{videoId}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();

            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Video '{videoId}' not found.");
        }

        public void Dispose()
            => _fixture.CleanPersistence();

    }
}
