using FC.CodeFlix.Catalog.Application.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.Common;
using FC.CodeFlix.Catalog.Application.UseCases.Video.ListVideos;
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
                outputItem.Rating.Should().Be(exampleItem.Rating);
                outputItem.Thumb!.Should().Be(exampleItem.Thumb!.Path);
                outputItem.ThumbHalf!.Should().Be(exampleItem.ThumbHalf!.Path);
                outputItem.Banner!.Should().Be(exampleItem.Banner!.Path);
                outputItem.Media!.Should().Be(exampleItem.Media!.FilePath);
                outputItem.Trailer!.Should().Be(exampleItem.Trailer!.FilePath);
                outputItem.CastMembers.Should().BeEquivalentTo(exampleItem.CastMembers);
                outputItem.Genres.Should().BeEquivalentTo(exampleItem.Genres);
                outputItem.Categories.Should().BeEquivalentTo(exampleItem.Categories);
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
