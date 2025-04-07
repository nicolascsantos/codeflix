namespace FC.CodeFlix.Catalog.UnitTests.Application.Genre.UpdateGenre
{
    public static class UpdateGenreDataGenerator
    {
        public static IEnumerable<object[]> GetGenresToUpdate(int times = 10)
        {
            var fixture = new UpdateGenreTestFixture();
            for (int index = 0; index < times; index++)
            {
                var exampleGenre = fixture.GetValidGenre();
                var exampleInput = fixture.GetExampleInput();
                yield return new object[] { exampleGenre, exampleInput };
            }
        }
    }
}
