using FC.CodeFlix.Catalog.Domain.Enum;

namespace FC.CodeFlix.Catalog.Domain.Extensions
{
    public static class RatingExtension
    {
        public static Rating ToRating(this string enumString)
            => enumString switch
            {
                "ER" => Rating.ER,
                "L" => Rating.L,
                "10" => Rating.RATE_10,
                "12" => Rating.RATE_12,
                "14" => Rating.RATE_14,
                "16" => Rating.RATE_16,
                "18" => Rating.RATE_18,
                _ => throw new ArgumentOutOfRangeException(nameof(enumString)),
            };
    }
}
