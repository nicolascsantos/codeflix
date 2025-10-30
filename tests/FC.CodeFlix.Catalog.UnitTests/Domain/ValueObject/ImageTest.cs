using FC.CodeFlix.Catalog.Domain.ValueObject;
using FC.CodeFlix.Catalog.UnitTests.Common;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.ValueObject
{
    public class ImageTest : BaseFixture
    {
        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Image - ValueObject")]
        public void Instantiate()
        {
            var path = Faker.Image.PicsumUrl();

            var image = new Image(path);

            image.Path.Should().Be(path);
        }

        [Fact(DisplayName = nameof(EqualsByPath))]
        [Trait("Domain", "Image - ValueObject")]
        public void EqualsByPath()
        {
            var path = Faker.Image.PicsumUrl();

            var image = new Image(path);
            var imageToCompare = new Image(path);

            var isItEqual = image == imageToCompare;

            isItEqual.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(DifferentByPath))]
        [Trait("Domain", "Image - ValueObject")]
        public void DifferentByPath()
        {
            var path = Faker.Image.PicsumUrl();

            var image = new Image(path);
            var imageToCompare = new Image(path);

            var isItEqual = image != imageToCompare;

            isItEqual.Should().BeFalse();
        }
    }
}
