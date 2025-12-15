using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.UpdateVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.UpdateVideo
{
    [CollectionDefinition(nameof(UpdateVideoTestFixture))]
    public class UpdateVideoTestFixtureCollection : ICollectionFixture<UpdateVideoTestFixture> { }

    public class UpdateVideoTestFixture : VideoTestFixtureBase
    {
        public UseCases.UpdateVideoInput GetValidInput(
            Guid videoId,
            List<Guid>? genresIds = null,
            List<Guid>? categoriesIds = null,
            List<Guid>? castMembersIds = null
        )
            => new(
               videoId,
               GetValidTitle(),
               GetValidDescription(),
               GetValidYearLaunched(),
               GetRandomBoolean(),
               GetRandomBoolean(),
               GetValidDuration(),
               GetRandomRating(),
               genresIds,
               categoriesIds,
               castMembersIds
            );
    }
}
