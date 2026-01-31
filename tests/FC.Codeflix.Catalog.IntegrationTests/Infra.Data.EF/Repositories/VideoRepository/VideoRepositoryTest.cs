using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
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
    }
}
