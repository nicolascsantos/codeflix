using FC.CodeFlix.Catalog.Application.UseCases.Genre.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.EndToEndTests.API.Video.Common;
using FC.CodeFlix.Catalog.EndToEndTests.Extensions.DateTime;
using FC.CodeFlix.Catalog.EndToEndTests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Video.ListVideos
{
    [Collection(nameof(VideoBaseFixture))]
    public class ListVideosAPITests : IDisposable
    {
        private readonly VideoBaseFixture _fixture;

        public ListVideosAPITests(VideoBaseFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(ListVideos))]
        [Trait("EndToEnd/API", "Video/List - Endpoints")]
        public async Task ListVideos()
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

            var input = new ListVideosInput();

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<VideoModelOutput>>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Data.Should().NotBeNull();
            output.Data.Count.Should().Be(exampleVideos.Count);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleVideos
                    .Find(x => x.Id == outputItem.Id);

                exampleItem.Should().NotBeNull();
                outputItem.Id.Should().Be(exampleItem.Id);
                outputItem.Title.Should().Be(exampleItem.Title);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
                outputItem.Opened.Should().Be(exampleItem.Opened);
                outputItem.Published.Should().Be(exampleItem.Published);
                outputItem.Duration.Should().Be(exampleItem.Duration);
                outputItem.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);

                var expectedCategories = exampleCategories
                    .Select(category =>
                        new VideoModelOutputRelatedAggregate(category.Id, category.Name)
                    );
                outputItem.Categories.Should().BeEquivalentTo(exampleCategories);

                var expectedGenres = exampleGenres
                    .Select(genre =>
                        new VideoModelOutputRelatedAggregate(genre.Id, genre.Name)
                    );
                outputItem.Genres.Should().BeEquivalentTo(exampleGenres);

                var expectedCastMembers = exampleCastMembers
                    .Select(castMember =>
                        new VideoModelOutputRelatedAggregate(castMember.Id, castMember.Name)
                    );
                outputItem.CastMembers.Should().BeEquivalentTo(exampleCastMembers);
            });
        }

        [Fact(DisplayName = nameof(ReturnsEmptyWhenThereIsNoVideo))]
        [Trait("EndToEnd/API", "Video/List - Endpoints")]
        public async Task ReturnsEmptyWhenThereIsNoVideo()
        {
            var input = new ListVideosInput();

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<VideoModelOutput>>("/api/videos", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.Total.Should().Be(0);
            output.Data.Should().NotBeNull();
            output.Data.Should().BeEmpty();
        }

        [Theory(DisplayName = nameof(ListGenresPaginated))]
        [Trait("EndToEnd/API", "Video/List - Endpoints")]
        [InlineData(10, 1, 5, 5)]
        [InlineData(10, 2, 5, 5)]
        [InlineData(7, 2, 5, 2)]
        [InlineData(7, 3, 5, 0)]
        public async Task ListGenresPaginated(
           int quantityToGenerate,
           int page,
           int perPage,
           int expectedQuantityItems
           )
        {
            var exampleVideos = _fixture.GetVideoCollection(quantityToGenerate);
            var exampleCategories = _fixture.GetExampleCategoriesList(3);
            var exampleGenres = _fixture.GetExampleListGenres(4);
            var exampleCastMembers = _fixture.GetExampleCastMembersList(5);
            await _fixture.VideoPersistence.InsertList(exampleVideos);

            var input = new ListVideosInput
            {
                Page = page,
                PerPage = perPage
            };

            var (response, output) = await _fixture.APIClient
                .Get<TestAPIResponseList<VideoModelOutput>>("/api/genres", input);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
            output.Should().NotBeNull();
            output.Meta.Should().NotBeNull();
            output.Data.Should().NotBeNull();
            output.Meta.Total.Should().Be(quantityToGenerate);
            output.Meta.CurrentPage.Should().Be(input.Page);
            output.Meta.PerPage.Should().Be(input.PerPage);
            output.Data.Count.Should().Be(expectedQuantityItems);
            output.Data.ToList().ForEach(outputItem =>
            {
                var exampleItem = exampleVideos
                    .Find(x => x.Id == outputItem.Id);

                exampleItem.Should().NotBeNull();
                outputItem.Id.Should().Be(exampleItem.Id);
                outputItem.Title.Should().Be(exampleItem.Title);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
                outputItem.Opened.Should().Be(exampleItem.Opened);
                outputItem.Published.Should().Be(exampleItem.Published);
                outputItem.Duration.Should().Be(exampleItem.Duration);
                outputItem.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);

                var expectedCategories = exampleCategories
                    .Select(category =>
                        new VideoModelOutputRelatedAggregate(category.Id, category.Name)
                    );
                outputItem.Categories.Should().BeEquivalentTo(exampleCategories);

                var expectedGenres = exampleGenres
                    .Select(genre =>
                        new VideoModelOutputRelatedAggregate(genre.Id, genre.Name)
                    );
                outputItem.Genres.Should().BeEquivalentTo(exampleGenres);

                var expectedCastMembers = exampleCastMembers
                    .Select(castMember =>
                        new VideoModelOutputRelatedAggregate(castMember.Id, castMember.Name)
                    );
                outputItem.CastMembers.Should().BeEquivalentTo(exampleCastMembers);
            });
        }

        public void Dispose()
            => _fixture.CleanPersistence();

    }
}
