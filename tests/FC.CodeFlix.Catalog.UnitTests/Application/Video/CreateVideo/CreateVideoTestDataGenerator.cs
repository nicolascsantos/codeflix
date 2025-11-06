using FC.CodeFlix.Catalog.Application.UseCases.Video.CreateVideo;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Video.CreateVideo
{
    public static class CreateVideoTestDataGenerator
    {
        public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
        {
            var fixture = new CreateVideoTestFixture();
            var invalidInputsList = new List<object[]>();
            const int totalInvalidCases = 2;

            for (int index = 0; index < times; index++)
            {
                switch (index % totalInvalidCases)
                {
                    case 0:
                        invalidInputsList.Add(
                        [
                            new CreateVideoInput(
                                string.Empty,
                                fixture.GetValidDescription(),
                                fixture.GetValidYearLaunched(),
                                fixture.GetRandomBoolean(),
                                fixture.GetRandomBoolean(),
                                fixture.GetValidDuration(),
                                fixture.GetRandomRating()
                            ),
                            "Title is required."
                        ]);
                        break;
                    case 1:
                        invalidInputsList.Add(
                        [
                            new CreateVideoInput(
                                fixture.GetValidTitle(),
                                string.Empty,
                                fixture.GetValidYearLaunched(),
                                fixture.GetRandomBoolean(),
                                fixture.GetRandomBoolean(),
                                fixture.GetValidDuration(),
                                fixture.GetRandomRating()
                            ),
                            "Description is required."
                        ]);
                        break;
                    case 2:
                        invalidInputsList.Add(
                        [
                            new CreateVideoInput(
                                fixture.GetTooLongTitle(),
                                fixture.GetValidDescription(),
                                fixture.GetValidYearLaunched(),
                                fixture.GetRandomBoolean(),
                                fixture.GetRandomBoolean(),
                                fixture.GetValidDuration(),
                                fixture.GetRandomRating()
                            ),
                            "Title should be less or equal 255 characters long."
                        ]);
                        break;
                    case 3:
                        invalidInputsList.Add(
                        [
                            new CreateVideoInput(
                                fixture.GetValidTitle(),
                                fixture.GetTooLongDescription(),
                                fixture.GetValidYearLaunched(),
                                fixture.GetRandomBoolean(),
                                fixture.GetRandomBoolean(),
                                fixture.GetValidDuration(),
                                fixture.GetRandomRating()
                            ),
                            "Description should be less or equal 255 characters long."
                        ]);
                        break;
                    default:
                        break;
                }
            }
            return invalidInputsList;
        }
    }
}
