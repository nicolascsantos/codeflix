using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.GetVideo
{
    [Collection(nameof(VideoBaseFixture))]
    public class GetVideoAPITest : IDisposable
    {
        private readonly VideoBaseFixture _fixture;

        public GetVideoAPITest(VideoBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(GetVideoById))]
        [Trait("EndToEnd/API", "Video/GetVideo - Endpoints")]
        public async Task GetVideoById()
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

            var exampleItem = exampleVideos.ElementAt(5);

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponse<VideoModelOutput>>($"/api/videos/{exampleItem.Id}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output.Data.Should().NotBeNull();

            output.Data.Id.Should().Be(exampleItem.Id);
            output.Data.Title.Should().Be(exampleItem.Title);
            output.Data.Description.Should().Be(exampleItem.Description);
            output.Data.YearLaunched.Should().Be(exampleItem.YearLaunched);
            output.Data.Opened.Should().Be(exampleItem.Opened);
            output.Data.Published.Should().Be(exampleItem.Published);
            output.Data.Duration.Should().Be(exampleItem.Duration);
            output.Data.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
            output.Data.CreatedAt.TrimMilliseconds().Should().Be(exampleItem.CreatedAt.TrimMilliseconds());

            var expectedCategories = exampleCategories
                .Select(category =>
                    new VideoModelOutputRelatedAggregate(category.Id, category.Name)
                );
            output.Data.Categories.Should().BeEquivalentTo(expectedCategories);

            var expectedGenres = exampleGenres
                .Select(genre =>
                    new VideoModelOutputRelatedAggregate(genre.Id, genre.Name)
                );
            output.Data.Genres.Should().BeEquivalentTo(expectedGenres);

            var expectedCastMembers = exampleCastMembers
                .Select(castMember =>
                    new VideoModelOutputRelatedAggregate(castMember.Id, castMember.Name)
                );
            output.Data.CastMembers.Should().BeEquivalentTo(expectedCastMembers);
        }

        [Fact(DisplayName = nameof(Error404WhenIdNotFound))]
        [Trait("EndToEnd/API", "Video/GetVideo - Endpoints")]
        public async Task Error404WhenIdNotFound()
        {
            var exampleVideos = _fixture.GetVideoCollection(10);
            
            await _fixture.VideoPersistence.InsertList(exampleVideos);

            var exampleVideoId = Guid.NewGuid();

            var (response, output) = await _fixture.APIClient
                .Get<ProblemDetails>($"/api/videos/{exampleVideoId}");

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            output.Should().NotBeNull();
            output.Type.Should().Be("NotFound");
            output.Detail.Should().Be($"Video '{exampleVideoId}' not found.");
        }

        public void Dispose() => _fixture.CleanPersistence();
    }
}
