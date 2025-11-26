using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.Extensions;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Domain.Extensions
{
    public class RatingExtensionsTest
    {
        [Theory(DisplayName = nameof(StringToRating))]
        [Trait("Domain", "Rating - Extensions")]
        [InlineData("ER", Rating.ER)]
        [InlineData("L", Rating.L)]
        [InlineData("10", Rating.RATE_10)]
        [InlineData("12", Rating.RATE_12)]
        [InlineData("14", Rating.RATE_14)]
        [InlineData("16", Rating.RATE_16)]
        [InlineData("18", Rating.RATE_18)]
        public void StringToRating(string enumString, Rating expectedRating)
            => enumString.ToRating().Should().Be(expectedRating);


        [Fact(DisplayName = nameof(ThrowsWhenInvalidString))]
        [Trait("Domain", "Rating - Extensions")]
        public void ThrowsWhenInvalidString()
        {
            var action = () => "Invalid".ToRating();
            action.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
