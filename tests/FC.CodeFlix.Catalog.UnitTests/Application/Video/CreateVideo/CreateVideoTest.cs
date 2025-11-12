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
            var storageService = new Mock<IStorageService>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
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

        [Fact(DisplayName = nameof(CreateVideoWithThumb))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithThumb()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var storageService = new Mock<IStorageService>();
            var expectedThumbName = $"thumb.jpg";

            storageService.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedThumbName);

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
            );

            var input = _fixture.GetValidInput(
                thumb: _fixture.GetValidImageFileInput()
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
            storageService.VerifyAll();

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
            output.Thumb.Should().Be(expectedThumbName);
        }

        [Fact(DisplayName = nameof(CreateVideoWithBanner))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithBanner()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var storageService = new Mock<IStorageService>();
            var expectedBannerName = $"banner.jpg";

            storageService.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedBannerName);

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
            );

            var input = _fixture.GetValidInput(
                banner: _fixture.GetValidImageFileInput()
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
            storageService.VerifyAll();

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
            output.Banner.Should().Be(expectedBannerName);
        }

        [Fact(DisplayName = nameof(CreateVideoWithThumbHalf))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithThumbHalf()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var storageService = new Mock<IStorageService>();
            var expectedThumbHalfName = $"thumbhalf.jpg";

            storageService.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedThumbHalfName);

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
            );

            var input = _fixture.GetValidInput(
                thumbHalf: _fixture.GetValidImageFileInput()
            );

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
            storageService.VerifyAll();

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
            output.ThumbHalf.Should().Be(expectedThumbHalfName);
        }

        [Fact(DisplayName = nameof(CreateVideoWithAllImages))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task CreateVideoWithAllImages()
        {
            var repositoryMock = new Mock<IVideoRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var storageService = new Mock<IStorageService>();
            var expectedBannerName = $"-banner.jpg";
            var expectedThumbName = "-thumb.jpg";
            var expectedThumbHalfName = "-thumbhalf.jpg";

            storageService.Setup(x => x.Upload(
                It.Is<string>(x => x.Contains("-banner")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedBannerName);

            storageService.Setup(x => x.Upload(
                It.Is<string>(x => x.Contains("-thumb")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedThumbName);

            storageService.Setup(x => x.Upload(
                It.Is<string>(x => x.Contains("-thumbhalf")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(expectedThumbHalfName);

            var categoryRepositoryMock = new Mock<ICategoryRepository>();
            var genreRepositoryMock = new Mock<IGenreRepository>();
            var castMemberRepositoryMock = new Mock<ICastMemberRepository>();


            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
            );

            var input = _fixture.GetValidInputWithAllImages();

            var output = await useCase.Handle(input, CancellationToken.None);

            repositoryMock.Verify(x => x.Insert(It.IsAny<DomainEntity.Video>(),
                It.IsAny<CancellationToken>())
            );

            unitOfWorkMock.Verify(x => x.Commit(It.IsAny<CancellationToken>()), Times.Once);
            storageService.VerifyAll();

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
            output.Banner.Should().Be(expectedBannerName);
            output.Thumb.Should().Be(expectedThumbName);
            output.ThumbHalf.Should().Be(expectedThumbHalfName);
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
            var storageService = new Mock<IStorageService>();

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
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
            var storageService = new Mock<IStorageService>();


            categoryRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<Guid> ids, CancellationToken ct) => ids);

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
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
            var storageService = new Mock<IStorageService>();


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
                castMemberRepositoryMock.Object,
                storageService.Object
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
            var storageService = new Mock<IStorageService>();

            var idsExamples = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

            genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples);

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
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
            var storageService = new Mock<IStorageService>();

            genreRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples.FindAll(x => x != idToRemove));

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
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
            var storageService = new Mock<IStorageService>();
            var idsExamples = Enumerable.Range(1, 5).Select(_ => Guid.NewGuid()).ToList();

            castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples);

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
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
            var storageService = new Mock<IStorageService>();

            castMemberRepositoryMock.Setup(x => x.GetIdsListByIds(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(idsExamples.FindAll(x => x != idToRemove));

            var useCase = new UseCases.CreateVideo(
                unitOfWorkMock.Object,
                repositoryMock.Object,
                categoryRepositoryMock.Object,
                genreRepositoryMock.Object,
                castMemberRepositoryMock.Object,
                storageService.Object
            );


            var input = _fixture.GetValidInput(castMembersIds: idsExamples);

            var action = async () => await useCase.Handle(input, CancellationToken.None);

            await action.Should()
                .ThrowAsync<RelatedAggregateException>($"Related CastMember id or ids not found: '{idToRemove}'");

            castMemberRepositoryMock.VerifyAll();
        }


        [Fact(DisplayName = nameof(ThrowsExceptionInUploadErrorCases))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsExceptionInUploadErrorCases()
        {
            
            var storageService = new Mock<IStorageService>();
            var expectedBannerName = $"-banner.jpg";

            storageService.Setup(x => x.Upload(
                It.IsAny<string>(),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ThrowsAsync(new Exception("Something went wrong with the upload."));

            var useCase = new UseCases.CreateVideo(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IVideoRepository>(),
                Mock.Of<ICategoryRepository>(),
                Mock.Of<IGenreRepository>(),
                Mock.Of<ICastMemberRepository>(),
                storageService.Object
            );

            var input = _fixture.GetValidInputWithAllImages();

            var action = async () 
                    => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong with the upload.");
        }

        [Fact(DisplayName = nameof(ThrowsExceptionAndRollbackInUploadErrorCases))]
        [Trait("Application", "CreateVideo - Use Cases")]
        public async Task ThrowsExceptionAndRollbackInUploadErrorCases()
        {

            var storageServiceMock = new Mock<IStorageService>();

            storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x.Contains("-banner")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync("-banner.jpg");

            storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x.Contains("-thumb")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync("-thumb.jpg");

            storageServiceMock.Setup(x => x.Upload(
                It.Is<string>(x => x.Contains("-thumbhalf")),
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>())
            ).ThrowsAsync(new Exception("Something went wrong with the upload."));

            var useCase = new UseCases.CreateVideo(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IVideoRepository>(),
                Mock.Of<ICategoryRepository>(),
                Mock.Of<IGenreRepository>(),
                Mock.Of<ICastMemberRepository>(),
                storageServiceMock.Object
            );

            var input = _fixture.GetValidInputWithAllImages();

            var action = async ()
                    => await useCase.Handle(input, CancellationToken.None);

            await action.Should().ThrowAsync<Exception>().WithMessage("Something went wrong with the upload.");

            storageServiceMock.Verify(x => x.Delete(It.Is<string>(x => x == "-banner.jpg" || x == "-thumb.jpg"), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
