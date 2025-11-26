using FC.CodeFlix.Catalog.Application.Common;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Common
{
    public class StoragePathNameTest
    {
        [Fact(DisplayName = nameof(CreateStorageNameForFile))]
        [Trait("Application", "StoragePathName - UseCases")]
        public void CreateStorageNameForFile()
        {
            var exampleId = Guid.NewGuid();
            var exampleExtension = "mp4";
            var propertyName = "Video";

            var name = StorageName.Create(exampleId, propertyName, exampleExtension);

            name.Should().NotBeNull();
            name.Should().Be($"{exampleId}-{propertyName}.{exampleExtension}");
        }
    }
}
