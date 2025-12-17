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
            dbVideo.CastMembers.Should().HaveCount(castMembers.Count());
            dbVideo.Categories.Should().HaveCount(categories.Count());
            dbVideo.Genres.Should().HaveCount(genres.Count());
            dbVideo.CastMembers.Should().BeEquivalentTo(castMembers.Select(castMember => castMember.Id));
            dbVideo.Categories.Should().BeEquivalentTo(categories.Select(categories => categories.Id));
            dbVideo.Genres.Should().BeEquivalentTo(genres.Select(genre => genre.Id));
        }
    }
}
