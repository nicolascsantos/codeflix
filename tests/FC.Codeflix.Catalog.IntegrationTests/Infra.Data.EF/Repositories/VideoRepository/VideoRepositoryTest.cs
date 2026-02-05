using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Context = FC.Codeflix.Catalog.Infra.Data.EF;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.VideoRepository
{
    [Collection(nameof(VideoRepositoryTestFixture))]
    public class VideoRepositoryTest
    {
        private readonly VideoRepositoryTestFixture _fixture;

        public VideoRepositoryTest(VideoRepositoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Insert))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Insert()
        {
            Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetExampleVideo();
            IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

            await videoRepository.Insert(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo.Id.Should().Be(exampleVideo.Id);
            dbVideo.Title.Should().Be(exampleVideo.Title);
            dbVideo.Description.Should().Be(exampleVideo.Description);
            dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            dbVideo.Opened.Should().Be(exampleVideo.Opened);
            dbVideo.Published.Should().Be(exampleVideo.Published);
            dbVideo.Duration.Should().Be(exampleVideo.Duration);
            dbVideo.Rating.Should().Be(exampleVideo.Rating);
            dbVideo.CreatedAt.Should().BeCloseTo(exampleVideo.CreatedAt, TimeSpan.FromSeconds(1));

            dbVideo.Thumb.Should().BeNull();
            dbVideo.ThumbHalf.Should().BeNull();
            dbVideo.Banner.Should().BeNull();

            dbVideo.Trailer.Should().BeNull();
            dbVideo.Media.Should().BeNull();

            dbVideo.Categories.Should().BeEmpty();
            dbVideo.Genres.Should().BeEmpty();
            dbVideo.CastMembers.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(InsertWithMediasAndImages))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task InsertWithMediasAndImages()
        {
            Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetValidVideoWithAllProperties();
            IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

            await videoRepository.Insert(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos
                .Include(x => x.Media)
                .Include(x => x.Trailer)
                .FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo.Id.Should().Be(exampleVideo.Id);
            dbVideo.Thumb.Should().NotBeNull();
            dbVideo.Thumb!.Path.Should().Be(exampleVideo.Thumb!.Path);
            dbVideo.ThumbHalf.Should().NotBeNull();
            dbVideo.ThumbHalf!.Path.Should().Be(exampleVideo.ThumbHalf!.Path);
            dbVideo.Banner.Should().NotBeNull();
            dbVideo.Banner!.Path.Should().Be(exampleVideo.Banner!.Path);
            dbVideo.Media.Should().NotBeNull();
            dbVideo.Media!.FilePath.Should().Be(exampleVideo.Media!.FilePath);
            dbVideo.Media.EncodedPath.Should().Be(exampleVideo.Media.EncodedPath);
            dbVideo.Media.Status.Should().Be(exampleVideo.Media.Status);
            dbVideo.Trailer.Should().NotBeNull();
            dbVideo.Trailer!.FilePath.Should().Be(exampleVideo.Trailer!.FilePath);
            dbVideo.Trailer.EncodedPath.Should().Be(exampleVideo.Trailer.EncodedPath);
            dbVideo.Trailer.Status.Should().Be(exampleVideo.Trailer.Status);

        }

        [Fact(DisplayName = nameof(InsertWithRelationships))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task InsertWithRelationships()
        {
            Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetExampleVideo();

            var castMembers = _fixture.GetRandomCastMembersList();
            await dbContext.CastMembers.AddRangeAsync(castMembers);
            castMembers.ToList()
                .ForEach(castMember => exampleVideo.AddCastMember(castMember.Id));

            var categories = _fixture.GetRandomCategoriesList();
            await dbContext.Categories.AddRangeAsync(categories);
            categories.ToList()
                .ForEach(category => exampleVideo.AddCategory(category.Id));

            var genres = _fixture.GetRandomGenresList();
            await dbContext.Genres.AddRangeAsync(genres);
            genres.ToList()
                .ForEach(genre => exampleVideo.AddGenre(genre.Id));

            await dbContext.SaveChangesAsync();

            IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

            await videoRepository.Insert(exampleVideo, CancellationToken.None);
            await dbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();

            var dbVideosCategories = assertsDbContext.VideosCategories
                .Where(relation => relation.VideoId == exampleVideo.Id)
                .ToList();
            dbVideosCategories.Should().HaveCount(categories.Count);
            dbVideosCategories.Select(relation => relation.CategoryId).ToList()
                .Should()
                .BeEquivalentTo(categories.Select(x => x.Id));

            var dbVideosGenres = assertsDbContext.VideosGenres
                .Where(relation => relation.VideoId == exampleVideo.Id)
                .ToList();
            dbVideosGenres.Should().HaveCount(genres.Count);
            dbVideosGenres.Select(relation => relation.GenreId).ToList()
                .Should()
                .BeEquivalentTo(genres.Select(x => x.Id));

            var dbVideosCastMembers = assertsDbContext.VideosCastMembers
                .Where(relation => relation.VideoId == exampleVideo.Id)
                .ToList();
            dbVideosCastMembers.Should().HaveCount(castMembers.Count);
            dbVideosCastMembers.Select(relation => relation.CastMemberId).ToList()
                .Should()
                .BeEquivalentTo(castMembers.Select(x => x.Id));
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Update()
        {
            // Arrange
            Context.CodeflixCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetExampleVideo();
            await dbContextArrange.AddAsync(exampleVideo);
            await dbContextArrange.SaveChangesAsync();
            var videoNewValues = _fixture.GetExampleVideo();
            var dbContextAct = _fixture.CreateDbContext(true);
            IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);

            //Act
            exampleVideo.Update(
                videoNewValues.Title,
                videoNewValues.Description,
                videoNewValues.YearLaunched,
                videoNewValues.Opened,
                videoNewValues.Published,
                videoNewValues.Duration,
                videoNewValues.Rating
            );

            await videoRepository.Update(exampleVideo, CancellationToken.None);
            await dbContextAct.SaveChangesAsync(CancellationToken.None);

            // Assert
            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();
            dbVideo.Id.Should().Be(exampleVideo.Id);
            dbVideo.Title.Should().Be(videoNewValues.Title);
            dbVideo.Description.Should().Be(videoNewValues.Description);
            dbVideo.YearLaunched.Should().Be(videoNewValues.YearLaunched);
            dbVideo.Opened.Should().Be(videoNewValues.Opened);
            dbVideo.Published.Should().Be(videoNewValues.Published);
            dbVideo.Duration.Should().Be(videoNewValues.Duration);
            dbVideo.Rating.Should().Be(videoNewValues.Rating);

            dbVideo.Thumb.Should().BeNull();
            dbVideo.ThumbHalf.Should().BeNull();
            dbVideo.Banner.Should().BeNull();

            dbVideo.Trailer.Should().BeNull();
            dbVideo.Media.Should().BeNull();

            dbVideo.Categories.Should().BeEmpty();
            dbVideo.Genres.Should().BeEmpty();
            dbVideo.CastMembers.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(UpdateEntitesAndValueObjects))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task UpdateEntitesAndValueObjects()
        {
            // Arrange
            Context.CodeflixCatalogDbContext dbContextArrange = _fixture.CreateDbContext();
            var exampleVideo = _fixture.GetExampleVideo();
            await dbContextArrange.AddAsync(exampleVideo);
            await dbContextArrange.SaveChangesAsync();
            var videoNewValues = _fixture.GetExampleVideo();
            var dbContextAct = _fixture.CreateDbContext(true);
            var updatedThumb = _fixture.GetValidImagePath();
            var updatedThumbHalf = _fixture.GetValidImagePath();
            var updatedBanner = _fixture.GetValidImagePath();
            var updatedMedia = _fixture.GetValidImagePath();
            var updatedMediaEncoded = _fixture.GetValidImagePath();
            var updatedTrailer = _fixture.GetValidImagePath();
            IVideoRepository videoRepository = new Repository.VideoRepository(dbContextAct);
            var savedVideo = dbContextAct.Videos.Single(video => video.Id == exampleVideo.Id);

            //Act
            savedVideo.UpdateThumb(updatedThumb);
            savedVideo.UpdateThumbHalf(updatedThumbHalf);
            savedVideo.UpdateBanner(updatedBanner);
            savedVideo.UpdateMedia(updatedMedia);
            savedVideo.UpdateTrailer(updatedTrailer);
            savedVideo.UpdateAsEncoded(updatedMediaEncoded);

            await videoRepository.Update(savedVideo, CancellationToken.None);
            await dbContextAct.SaveChangesAsync(CancellationToken.None);

            // Assert
            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FirstOrDefaultAsync(video => video.Id == exampleVideo.Id);

            dbVideo.Should().NotBeNull();

            dbVideo.Thumb.Should().NotBeNull();
            dbVideo.Thumb.Path.Should().Be(updatedThumb);

            dbVideo.ThumbHalf.Should().NotBeNull();
            dbVideo.ThumbHalf.Path.Should().Be(updatedThumbHalf);

            dbVideo.Banner.Should().NotBeNull();
            dbVideo.Banner.Path.Should().Be(updatedBanner);

            dbVideo.Media.Should().NotBeNull();
            dbVideo.Media!.FilePath.Should().Be(updatedMedia);
            dbVideo.Media.EncodedPath.Should().Be(updatedMediaEncoded);
            dbVideo.Media.Status.Should().Be(MediaStatus.COMPLETED);

            dbVideo.Trailer.Should().NotBeNull();
            dbVideo.Trailer.FilePath.Should().Be(updatedTrailer);

            dbVideo.Categories.Should().BeEmpty();
            dbVideo.Genres.Should().BeEmpty();
            dbVideo.CastMembers.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(UpdateWithRelationships))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task UpdateWithRelationships()
        {
            var id = Guid.Empty;
            var castMembers = _fixture.GetRandomCastMembersList();
            var categories = _fixture.GetRandomCategoriesList();
            var genres = _fixture.GetRandomGenresList();

            using (Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext())
            {
                var exampleVideo = _fixture.GetExampleVideo();
                id = exampleVideo.Id;
                await dbContext.Videos.AddAsync(exampleVideo);


                await dbContext.CastMembers.AddRangeAsync(castMembers);
                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.Genres.AddRangeAsync(genres);
                await dbContext.SaveChangesAsync();
            }

            var actDbContext = _fixture.CreateDbContext(true);
            var savedVideo = await actDbContext.Videos
                .FirstAsync(video => video.Id == id);

            IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

            castMembers.ToList()
                .ForEach(castMember => savedVideo.AddCastMember(castMember.Id));

            categories.ToList()
                .ForEach(category => savedVideo.AddCategory(category.Id));

            genres.ToList()
                .ForEach(genre => savedVideo.AddGenre(genre.Id));

            await videoRepository.Update(savedVideo, CancellationToken.None);
            await actDbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(id);
            dbVideo.Should().NotBeNull();

            var dbVideosCategories = assertsDbContext.VideosCategories
                .Where(relation => relation.VideoId == id)
                .ToList();
            dbVideosCategories.Should().HaveCount(categories.Count);
            dbVideosCategories.Select(relation => relation.CategoryId).ToList()
                .Should()
                .BeEquivalentTo(categories.Select(x => x.Id));

            var dbVideosGenres = assertsDbContext.VideosGenres
                .Where(relation => relation.VideoId == id)
                .ToList();
            dbVideosGenres.Should().HaveCount(genres.Count);
            dbVideosGenres.Select(relation => relation.GenreId).ToList()
                .Should()
                .BeEquivalentTo(genres.Select(x => x.Id));

            var dbVideosCastMembers = assertsDbContext.VideosCastMembers
                .Where(relation => relation.VideoId == id)
                .ToList();
            dbVideosCastMembers.Should().HaveCount(castMembers.Count);
            dbVideosCastMembers.Select(relation => relation.CastMemberId).ToList()
                .Should()
                .BeEquivalentTo(castMembers.Select(x => x.Id));
        }

        [Fact(DisplayName = nameof(Delete))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Delete()
        {
            var id = Guid.Empty;

            using (Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext())
            {
                var exampleVideo = _fixture.GetExampleVideo();
                id = exampleVideo.Id;
                await dbContext.Videos.AddAsync(exampleVideo);
                await dbContext.SaveChangesAsync();
            }

            var actDbContext = _fixture.CreateDbContext(true);
            var savedVideo = await actDbContext.Videos
                .FirstAsync(video => video.Id == id);

            IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

            await videoRepository.Delete(savedVideo, CancellationToken.None);
            await actDbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(id);

            dbVideo.Should().BeNull();
        }

        [Fact(DisplayName = nameof(DeleteWithAllPropertiesAndRelationships))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task DeleteWithAllPropertiesAndRelationships()
        {
            var id = Guid.Empty;
            var castMembers = _fixture.GetRandomCastMembersList();
            var categories = _fixture.GetRandomCategoriesList();
            var genres = _fixture.GetRandomGenresList();

            using (Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext())
            {
                var exampleVideo = _fixture.GetValidVideoWithAllProperties();
                id = exampleVideo.Id;

                castMembers.ToList()
                    .ForEach(castMember => dbContext.VideosCastMembers.Add(new VideosCastMembers(id, castMember.Id)));

                categories.ToList()
                    .ForEach(category => dbContext.VideosCategories.Add(new VideosCategories(id, category.Id)));

                genres.ToList()
                    .ForEach(genre => dbContext.VideosGenres.Add(new VideosGenres(id, genre.Id)));

                await dbContext.CastMembers.AddRangeAsync(castMembers);
                await dbContext.Categories.AddRangeAsync(categories);
                await dbContext.Genres.AddRangeAsync(genres);
                await dbContext.Videos.AddAsync(exampleVideo);
                await dbContext.SaveChangesAsync();
            }

            var actDbContext = _fixture.CreateDbContext(true);
            var savedVideo = await actDbContext.Videos
                .FirstAsync(video => video.Id == id);

            IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

            await videoRepository.Delete(savedVideo, CancellationToken.None);
            await actDbContext.SaveChangesAsync(CancellationToken.None);

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(id);

            dbVideo.Should().BeNull();

            assertsDbContext.VideosCategories
                .Where(relation => relation.VideoId == id)
                .ToList().Count().Should().Be(0);

            assertsDbContext.VideosGenres
                 .Where(relation => relation.VideoId == id)
                 .ToList().Count().Should().Be(0);

            assertsDbContext.VideosCastMembers
                .Where(relation => relation.VideoId == id)
                .ToList().Count().Should().Be(0);

            assertsDbContext.Set<Media>().Count().Should().Be(0);
        }

        [Fact(DisplayName = nameof(Get))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task Get()
        {
            var exampleVideo = _fixture.GetExampleVideo();

            using (Context.CodeflixCatalogDbContext dbContext = _fixture.CreateDbContext())
            {
                await dbContext.Videos.AddAsync(exampleVideo);
                await dbContext.SaveChangesAsync();
            }

            var actDbContext = _fixture.CreateDbContext(true);
            var savedVideo = await actDbContext.Videos
                .FirstAsync(video => video.Id == exampleVideo.Id);

            IVideoRepository videoRepository = new Repository.VideoRepository(actDbContext);

            await videoRepository.Get(exampleVideo.Id, CancellationToken.None);
            

            var assertsDbContext = _fixture.CreateDbContext(true);
            var dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);

            dbVideo.Should().NotBeNull();
            dbVideo.Title.Should().Be(exampleVideo.Title);
            dbVideo.Description.Should().Be(exampleVideo.Description);
            dbVideo.YearLaunched.Should().Be(exampleVideo.YearLaunched);
            dbVideo.Opened.Should().Be(exampleVideo.Opened);
            dbVideo.Published.Should().Be(exampleVideo.Published);
            dbVideo.Duration.Should().Be(exampleVideo.Duration);
            dbVideo.Rating.Should().Be(exampleVideo.Rating);
            dbVideo.CreatedAt.Should().Be(exampleVideo.CreatedAt);
            dbVideo.Thumb.Should().BeNull();
            dbVideo.ThumbHalf.Should().BeNull();
            dbVideo.Banner.Should().BeNull();
            dbVideo.Media.Should().BeNull();
            dbVideo.Trailer.Should().BeNull();
        }

        [Fact(DisplayName = nameof(GetThrowIfNotFound))]
        [Trait("Integration/Infra.Data", "VideoRepository - Repositories")]
        public async Task GetThrowIfNotFound()
        {
            var dbContext = _fixture.CreateDbContext();
            var randomGuid = Guid.NewGuid();
            IVideoRepository videoRepository = new Repository.VideoRepository(dbContext);

            var action = async () => await videoRepository.Get(randomGuid, CancellationToken.None);

            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage($"Video '{randomGuid}' not found.");
        }
    }
}
