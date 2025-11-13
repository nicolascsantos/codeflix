using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.UnitTests.Common.Fixtures;
using Moq;
using UseCases = FC.CodeFlix.Catalog.Application.UseCases.Video.UploadMedias;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.UploadMedias
{
    [CollectionDefinition(nameof(UploadMediasTestFixture))]
    public class UploadMediasTestFixtureCollection : ICollectionFixture<UploadMediasTestFixture> { }

    public class UploadMediasTestFixture : VideoTestFixtureBase
    {
        public UploadMediasInput GetValidUploadMediasInput(Guid? videoId = null)
            => new(
                videoId ?? Guid.NewGuid(),
                GetValidMediaFileInput(),
                GetValidMediaFileInput()
            );

        public UseCases.UploadMedias CreateUseCase()
            => new(
                Mock.Of<IVideoRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IStorageService>()
            );
    }
}
