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
        private readonly Mock<ICastMemberRepository> _castMemberRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UseCases.UpdateVideo _useCase;

        public UpdateVideoTest(UpdateVideoTestFixture fixture)
        {
            _fixture = fixture;
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _castMemberRepositoryMock = new Mock<ICastMemberRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _useCase = new UseCases.UpdateVideo(
                _videoRepositoryMock.Object,
                _genreRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _castMemberRepositoryMock.Object,
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

        [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidCategoriesIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenInvalidCategoriesIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var categoriesIdsExamples = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var invalidCategoryId = Guid.NewGuid();
            var invalidInputIdsList = categoriesIdsExamples.Concat(new List<Guid>() { invalidCategoryId }).ToList();
            var input = _fixture.GetValidInput(exampleVideo.Id, categoriesIds: invalidInputIdsList);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(categoriesIdsExamples);

            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id or ids not found: '{invalidCategoryId}'");

            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithCastMembersIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithCastMembersIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var exampleIds = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var input = _fixture.GetValidInput(exampleVideo.Id, castMembersIds: exampleIds);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleIds);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

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
                    (video.CastMembers.All(castMemberId => exampleIds.Contains(castMemberId))) &&
                    (video.CastMembers.Count == exampleIds.Count)),
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
            output.CastMembers.Select(category => category.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(exampleIds);
        }

        [Fact(DisplayName = nameof(UpdateVideosThrowsWhenInvalidCastMembersIds))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosThrowsWhenInvalidCastMembersIds()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var castMembersIdsExamples = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var invalidCastMemberId = Guid.NewGuid();
            var invalidInputIdsList = castMembersIdsExamples.Concat(new List<Guid>() { invalidCastMemberId }).ToList();
            var input = _fixture.GetValidInput(exampleVideo.Id, castMembersIds: invalidInputIdsList);

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(castMembersIdsExamples);

            var action = async () => await _useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related cast member id or ids not found: '{invalidCastMemberId}'");

            _videoRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();
            _unitOfWorkMock.Verify(x => x.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Never);
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

        [Fact(DisplayName = nameof(UpdateVideosWithoutRelationshipsWithRelationships))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithoutRelationshipsWithRelationships()
        {
            var exampleVideo = _fixture.GetValidVideo();
            var categoriesIdsExamples = Enumerable.Range(1, 5)
               .Select(_ => Guid.NewGuid()).ToList();
            var genresIdsExamples = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var castmembersIdsExamples = Enumerable.Range(1, 5)
               .Select(_ => Guid.NewGuid()).ToList();


            var input = _fixture.GetValidInput(
                exampleVideo.Id,
                categoriesIds: categoriesIdsExamples,
                genresIds: genresIdsExamples,
                castMembersIds: castmembersIdsExamples
            );

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(categoriesIdsExamples);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(genresIdsExamples);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(castmembersIdsExamples);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

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
                    (video.Genres.All(genreId => genresIdsExamples.Contains(genreId))) &&
                    (video.Genres.Count == genresIdsExamples.Count) &&
                    (video.Categories.All(categoryId => categoriesIdsExamples.Contains(categoryId))) &&
                    (video.Categories.Count == categoriesIdsExamples.Count) && 
                    (video.CastMembers.All(castMemberId => castmembersIdsExamples.Contains(castMemberId))) &&
                    (video.CastMembers.Count == castmembersIdsExamples.Count)),
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
                .BeEquivalentTo(categoriesIdsExamples);
            output.Genres.Select(genre => genre.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(genresIdsExamples);
            output.CastMembers.Select(castMember => castMember.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(castmembersIdsExamples);
        }

        [Fact(DisplayName = nameof(UpdateVideosWithoutRelationshipsWithOtherRelationships))]
        [Trait("Application", "UpdateVideo - Use Cases")]
        public async Task UpdateVideosWithoutRelationshipsWithOtherRelationships()
        {
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            var categoriesIdsExamples = Enumerable.Range(1, 5)
               .Select(_ => Guid.NewGuid()).ToList();
            var genresIdsExamples = Enumerable.Range(1, 5)
                .Select(_ => Guid.NewGuid()).ToList();
            var castmembersIdsExamples = Enumerable.Range(1, 5)
               .Select(_ => Guid.NewGuid()).ToList();


            var input = _fixture.GetValidInput(
                exampleVideo.Id,
                categoriesIds: categoriesIdsExamples,
                genresIds: genresIdsExamples,
                castMembersIds: castmembersIdsExamples
            );

            _videoRepositoryMock.Setup(x => x.Get(
                It.Is<Guid>(videoId => videoId == exampleVideo.Id),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(exampleVideo);

            _categoryRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(categoriesIdsExamples);

            _genreRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(genresIdsExamples);

            _castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(
                It.IsAny<List<Guid>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(castmembersIdsExamples);

            var output = await _useCase.Handle(input, CancellationToken.None);

            _videoRepositoryMock.VerifyAll();
            _categoryRepositoryMock.VerifyAll();
            _genreRepositoryMock.VerifyAll();
            _castMemberRepositoryMock.VerifyAll();

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
                    (video.Genres.All(genreId => genresIdsExamples.Contains(genreId))) &&
                    (video.Genres.Count == genresIdsExamples.Count) &&
                    (video.Categories.All(categoryId => categoriesIdsExamples.Contains(categoryId))) &&
                    (video.Categories.Count == categoriesIdsExamples.Count) &&
                    (video.CastMembers.All(castMemberId => castmembersIdsExamples.Contains(castMemberId))) &&
                    (video.CastMembers.Count == castmembersIdsExamples.Count)),
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
                .BeEquivalentTo(categoriesIdsExamples);
            output.Genres.Select(genre => genre.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(genresIdsExamples);
            output.CastMembers.Select(castMember => castMember.Id)
                .ToList()
                .Should()
                .BeEquivalentTo(castmembersIdsExamples);
        }
    }
}
