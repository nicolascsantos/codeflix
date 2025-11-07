using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;
using FC.CodeFlix.Catalog.Domain.Exceptions;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Moq;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.CreateVideo
{
    [Collection(nameof(CreateVideoTestFixture))]
    public class CreateVideoTest
    {
        private readonly CreateVideoTestFixture _fixture;

        public CreateVideoTest(CreateVideoTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(CreateVideo))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideo()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );

            var input = _fixture.GetValidInput();

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating);
            output.CreatedAt.Should().NotBeSameDateAs(default);
        }

        [Theory(DisplayName = nameof(CreateVideoThrowsWhenInputIsInvalid))]
        [Trait("Application", "CreateVideo - Use Cases")]
        [MemberData(nameof(CreateVideoTestDataGenerator.GetInvalidInputs), parameters: 12, MemberType = typeof(CreateVideoTestDataGenerator))]
        public async Task CreateVideoThrowsWhenInputIsInvalid(CreateVideoInput input, string expectedValidationMessage)
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            (await action.Should().ThrowAsync<EntityValidationException>()
                .WithMessage($"There are validation errors")).Which.Errors!.ToList()[0].Message.Should().Be(expectedValidationMessage);

            repositoryMock.Verify(
                x => x.Insert(It.IsAny<DomainEntity.Video>(), It.IsAny<CancellationToken>()), Times.Never);

        }

        [Fact(DisplayName = nameof(CreateVideoWithCategoriesIds))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithCategoriesIds()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<Guid> ids, CancellationToken ct) => ids);

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );

            var categoriesIdsExample = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

            var input = _fixture.GetValidInput(categoriesIdsExample);

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(
                video => video.Opened == input.Opened &&
                         video.Published == input.Published &&
                         video.Title == input.Title &&
                         video.Description == input.Description &&
                         video.YearLaunched == input.YearLaunched &&
                         video.Duration == input.Duration &&
                         video.Rating == input.Rating &&
                         video.Categories.All(categoryId => categoriesIdsExample.Contains(categoryId))
                ),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating);
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Categories.Should().BeEquivalentTo(categoriesIdsExample);

            repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(
                video => video.Opened == input.Opened &&
                         video.Published == input.Published &&
                         video.Title == input.Title &&
                         video.Description == input.Description &&
                         video.YearLaunched == input.YearLaunched &&
                         video.Duration == input.Duration &&
                         video.Rating == input.Rating &&
                         video.Categories.All(categoryId => categoriesIdsExample.Contains(categoryId))
                ),
                It.IsAny<CancellationToken>())
            );
        }

        [Fact(DisplayName = nameof(ThrowsWhenCategoryIdIsInvalid))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsWhenCategoryIdIsInvalid()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var categoriesIdsExample = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
            var categoryIdToRemove = categoriesIdsExample[2];
            categoryRepositoryMock
                .Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoriesIdsExample.FindAll(x => x != categoryIdToRemove));

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );


            var input = _fixture.GetValidInput(categoriesIdsExample);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>()
                .WithMessage($"Related category id or ids not found: '{categoryIdToRemove}'");

            repositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(CreateVideoWithGenresIds))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithGenresIds()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();

            var idsExamples = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

            genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples);

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );


            var input = _fixture.GetValidInput(genresIds: idsExamples);

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(
                video => video.Opened == input.Opened &&
                         video.Published == input.Published &&
                         video.Title == input.Title &&
                         video.Description == input.Description &&
                         video.YearLaunched == input.YearLaunched &&
                         video.Duration == input.Duration &&
                         video.Rating == input.Rating &&
                         video.Genres.All(genreId => idsExamples.Contains(genreId))
                ),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating);
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Categories.Should().BeEmpty();
            output.Genres.Should().BeEquivalentTo(idsExamples);

            genreRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ThrowsWhenGenreIdIsInvalid))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsWhenGenreIdIsInvalid()
        {
            var idsExamples = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
            var idToRemove = idsExamples[2];
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();

            genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples.FindAll(x => x != idToRemove));

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );


            var input = _fixture.GetValidInput(genresIds: idsExamples);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>($"Related genre id or ids not found: '{idToRemove}'");

            genreRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(CreateVideoWithCastMemberIds))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithCastMemberIds()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();
            var idsExamples = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

            castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples);

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );


            var input = _fixture.GetValidInput(castMembersIds: idsExamples);

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.Is<DomainEntity.Video>(
                video => video.Opened == input.Opened &&
                         video.Published == input.Published &&
                         video.Title == input.Title &&
                         video.Description == input.Description &&
                         video.YearLaunched == input.YearLaunched &&
                         video.Duration == input.Duration &&
                         video.Rating == input.Rating &&
                         video.CastMembers.All(genreId => idsExamples.Contains(genreId))
                ),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);

            output.Should().NotBeNull();
            output.Id.Should().NotBeEmpty();
            output.Title.Should().Be(input.Title);
            output.Description.Should().Be(input.Description);
            output.YearLaunched.Should().Be(input.YearLaunched);
            output.Opened.Should().Be(input.Opened);
            output.Published.Should().Be(input.Published);
            output.Duration.Should().Be(input.Duration);
            output.Rating.Should().Be(input.Rating);
            output.CreatedAt.Should().NotBeSameDateAs(default);
            output.Categories.Should().BeEmpty();
            output.Genres.Should().BeEmpty();
            output.CastMembers.Should().BeEquivalentTo(idsExamples);

            castMemberRepositoryMock.VerifyAll();
        }

        [Fact(DisplayName = nameof(ThrowsWhenCastMemberIdIsInvalid))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsWhenCastMemberIdIsInvalid()
        {
            var idsExamples = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();
            var idToRemove = idsExamples[2];
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();

            castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples.FindAll(x => x != idToRemove));

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object
            );


            var input = _fixture.GetValidInput(castMembersIds: idsExamples);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>($"Related CastMember id or ids not found: '{idToRemove}'");

            castMemberRepositoryMock.VerifyAll();
        }
    }
}
