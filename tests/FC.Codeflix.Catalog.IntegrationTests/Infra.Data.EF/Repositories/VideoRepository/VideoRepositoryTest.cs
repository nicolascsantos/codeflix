using FC.CodeFlix.Catalog.Domain.Repository;
using FluentAssertions;
using Context = FC.Codeflix.Catalog.Infra.Data.EF;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;
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
            DomainEntity.Video dbVideo = await assertsDbContext.Videos.FindAsync(exampleVideo.Id);
            dbVideo.Should().NotBeNull();
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
    }
}
