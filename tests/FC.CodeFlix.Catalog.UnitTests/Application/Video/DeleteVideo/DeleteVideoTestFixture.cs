using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.DeleteVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.DeleteVideo
{
    [CollectionDefinition(nameof(DeleteVideoTestFixture))]
    public class DeleteVideoTestFixtureCollection : ICollectionFixture<DeleteVideoTestFixture> { }

    public class DeleteVideoTestFixture : VideoTestFixtureBase
    {
        internal UseCases.DeleteVideoInput GetValidDeleteVideoInput(Guid? id = null)
            => new(id ?? Guid.NewGuid());
    }
}
