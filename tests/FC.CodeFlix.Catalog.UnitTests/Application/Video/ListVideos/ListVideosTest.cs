using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using FluentAssertions;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.ListVideos
{
    [Collection(nameof(ListVideosTestFixture))]
    public class ListVideosTest
    {
        private readonly ListVideosTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly UseCases.ListVideos _useCase;

        public ListVideosTest(ListVideosTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _useCase = new UseCases.ListVideos(_videoRepositoryMock.Object);
        }

        [Fact(DisplayName = nameof(ListVideos))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ListVideos()
        {
            var videosExampleList = _fixture.GetVideosExamplesList();
            var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);

            _videoRepositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(x => 
                    x.Page == input.Page && 
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new SearchOutput<Catalog.Domain.Entity.Video>(
                input.Page,
                input.PerPage,
                videosExampleList.Count,
                videosExampleList)
            );

            PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.Should().HaveCount(videosExampleList.Count);
            output.Total.Should().Be(videosExampleList.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var exampleItem = videosExampleList.Find(x => x.Id == outputItem.Id);
                exampleItem!.Should().NotBeNull();
                outputItem.Id.Should().Be(exampleItem.Id);
                outputItem.Title.Should().Be(exampleItem!.Title);
                outputItem.Description.Should().Be(exampleItem.Description);
                outputItem.YearLaunched.Should().Be(exampleItem.YearLaunched);
                outputItem.Opened.Should().Be(exampleItem.Opened);
                outputItem.Duration.Should().Be(exampleItem.Duration);
                outputItem.Published.Should().Be(exampleItem.Published);
                outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
                outputItem.Rating.Should().Be(exampleItem.Rating.ToStringSignal());
                outputItem.ThumbFileUrl!.Should().Be(exampleItem.Thumb!.Path);
                outputItem.ThumbHalfFileUrl!.Should().Be(exampleItem.ThumbHalf!.Path);
                outputItem.BannerFileUrl!.Should().Be(exampleItem.Banner!.Path);
                outputItem.VideoFileUrl!.Should().Be(exampleItem.Media!.FilePath);
                outputItem.TrailerFileUrl!.Should().Be(exampleItem.Trailer!.FilePath);
                outputItem.CastMembersIds.Should().BeEquivalentTo(exampleItem.CastMembers);
                outputItem.GenresIds.Should().BeEquivalentTo(exampleItem.Genres);
                outputItem.CategoriesIds.Should().BeEquivalentTo(exampleItem.Categories);
            });
            _videoRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ListVideosWithRelations))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ListVideosWithRelations()
        {
            var videosExampleList = _fixture.GetVideosExamplesList();
            var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);

            _videoRepositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new SearchOutput<Catalog.Domain.Entity.Video>(
                input.Page,
                input.PerPage,
                videosExampleList.Count,
                videosExampleList)
            );

            PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.Should().HaveCount(videosExampleList.Count);
            output.Total.Should().Be(videosExampleList.Count);
            output.Items.ToList().ForEach(outputItem =>
            {
                var videoExample = videosExampleList.Find(x => x.Id == outputItem.Id);
                videoExample!.Should().NotBeNull();
                outputItem.Id.Should().Be(videoExample.Id);
                outputItem.Title.Should().Be(videoExample!.Title);
                outputItem.Description.Should().Be(videoExample.Description);
                outputItem.YearLaunched.Should().Be(videoExample.YearLaunched);
                outputItem.Opened.Should().Be(videoExample.Opened);
                outputItem.Duration.Should().Be(videoExample.Duration);
                outputItem.Published.Should().Be(videoExample.Published);
                outputItem.CreatedAt.Should().Be(videoExample.CreatedAt);
                outputItem.Rating.Should().Be(videoExample.Rating.ToStringSignal());
                outputItem.ThumbFileUrl!.Should().Be(videoExample.Thumb!.Path);
                outputItem.ThumbHalfFileUrl!.Should().Be(videoExample.ThumbHalf!.Path);
                outputItem.BannerFileUrl!.Should().Be(videoExample.Banner!.Path);
                outputItem.VideoFileUrl!.Should().Be(videoExample.Media!.FilePath);
                outputItem.TrailerFileUrl!.Should().Be(videoExample.Trailer!.FilePath);
                outputItem.CastMembersIds.Should().BeEquivalentTo(videoExample.CastMembers);
                outputItem.GenresIds.Should().BeEquivalentTo(videoExample.Genres);
                outputItem.CategoriesIds.Should().BeEquivalentTo(videoExample.Categories);

                List<Guid> outputCategoriesIds =  outputItem.Categories
                    .Select(categoryDto => categoryDto.Id).ToList();
                outputCategoriesIds.Should().BeEquivalentTo(videoExample.Categories);

                List<Guid> outputGenresIds = outputItem.Genres
                    .Select(genresDto => genresDto.Id).ToList();
                outputGenresIds.Should().BeEquivalentTo(videoExample.Genres);

                List<Guid> outputCastMembersIds = outputItem.CastMembers
                    .Select(castMemberDto => castMemberDto.Id).ToList();
                outputCastMembersIds.Should().BeEquivalentTo(videoExample.CastMembers);
            });
            _videoRepositoryMock.VerifyAll();
        }


        [Fact(DisplayName = nameof(ListReturnsEmptyWhenThereIsNoVideo))]
        [Trait("Application", "ListVideos - Use Cases")]
        public async Task ListReturnsEmptyWhenThereIsNoVideo()
        {
            var input = new ListVideosInput(1, 10, "", "", SearchOrder.Asc);

            _videoRepositoryMock.Setup(x => x.Search(
                It.Is<SearchInput>(x =>
                    x.Page == input.Page &&
                    x.PerPage == input.PerPage &&
                    x.Search == input.Search &&
                    x.OrderBy == input.Sort &&
                    x.Order == input.Dir),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new SearchOutput<DomainEntity.Video>(
                input.Page,
                input.PerPage,
                0,
                new List<DomainEntity.Video>())
            );

            PaginatedListOutput<VideoModelOutput> output = await _useCase.Handle(input, CancellationToken.None);

            output.Should().NotBeNull();
            output.Page.Should().Be(input.Page);
            output.PerPage.Should().Be(input.PerPage);
            output.Items.Should().HaveCount(0);
            output.Total.Should().Be(0);
            _videoRepositoryMock.VerifyAll();
        }
    }
}
