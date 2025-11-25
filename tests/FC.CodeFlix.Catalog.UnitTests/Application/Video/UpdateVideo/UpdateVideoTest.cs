using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.UpdateVideo
{
    [Collection(nameof(UpdateVideoTestFixture))]
    public class UpdateVideoTest
    {
        private readonly UpdateVideoTestFixture _fixture;
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<IGenreRepository> _genreRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UseCases.UpdateVideo _useCase;

        public UpdateVideoTest(UpdateVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _useCase = new UseCases.UpdateVideo(
                _videoRepositoryMock.Object,
                _genreRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact(DisplayName = nameof(UpdateVideosBasicInfo))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosBasicInfo()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var input = _fixture.GetValidInput(exampleVideo.Id);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<DomainEntity.Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Rating == input.Rating)),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

            output.Should().NotBeNull();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
        }

        [Fact(DisplayName = nameof(UpdateVideosWithGenreId))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithGenreId()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var genreIdsExamples = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var input = _fixture.GetValidInput(exampleVideo.Id, genreIdsExamples);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(genreIdsExamples);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<DomainEntity.Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Rating == input.Rating) &&
                    (video.Genres.All(genreId => genreIdsExamples.Contains(genreId))) &&
                    (video.Genres.Count == genreIdsExamples.Count)),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

            output.Should().NotBeNull();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.Genres.Select(genre => genre.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(genreIdsExamples);
        }

        [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidGenreIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenInvalidGenreIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var genreIdsExamples = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var invalidGenresId = Guid.NewGuid();
            var invalidInputIdsList = genreIdsExamples.Concat(new List<Guid>() { invalidGenresId }).ToList();
            var input = _fixture.GetValidInput(exampleVideo.Id, invalidInputIdsList);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(genreIdsExamples);

            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related genre id or ids not found: '{invalidGenresId}'");

            _videoRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();
            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithCategoriesIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithCategoriesIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleIds = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var input = _fixture.GetValidInput(exampleVideo.Id, categoriesIds: exampleIds);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleIds);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();

            _videoRepositoryMock.Verify(repository => repository.Update(
                It.Is<DomainEntity.Video>(video =>
                    (video.Id == exampleVideo.Id) &&
                    (video.Title == input.Title) &&
                    (video.Description == input.Description) &&
                    (video.YearLaunched == input.YearLaunched) &&
                    (video.Opened == input.Opened) &&
                    (video.Published == input.Published) &&
                    (video.Duration == input.Duration) &&
                    (video.Rating == input.Rating) &&
                    (video.Categories.All(categoryId => exampleIds.Contains(categoryId))) &&
                    (video.Categories.Count == exampleIds.Count)),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()));

            output.Should().NotBeNull();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating.ToStringSignal());
            output.Categories.Select(category => category.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(exampleIds);
        }

        [Theory(DisplayName = nameof(UpdateVideosThrowsWhenRecieveInvalidInput))]
        [ClassData(typeof(UpdateVideoTestDataGenerator))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenRecieveInvalidInput(
            UpdateVideoInput invalidInput,
            string expectedExceptionMessage
        )
        {
            var exampleVideo = _fixture.GetValidVideo();

            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            var action = async () => await _useCase.Handle(invalidInput, CancellationToken.None);

            var exceptionAssertion = await action.Should()
                .ThrowAsync<EntityValidationException>()
                .WithMessage("There are validation errors.");

            exceptionAssertion.Which.Errors!
                .ToList()[0].Message.Should().Be(expectedExceptionMessage);

            _videoRepositoryMock.VerifyAll();
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = nameof(UpdateVideosThrowsWhenVideoNotFound))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenVideoNotFound()
        {
            var input = _fixture.GetValidInput(Guid.NewGuid());

            _videoRepositoryMock.Setup(x => x.Get(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new NotFoundException("Video not found."));

            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Video not found.");

            _videoRepositoryMock.Verify(x => x.Update(
                It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
            _unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
