using FC.CodeFlix.Catalog.API.APIModels.Category;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.Category.UpdateCategory
{
    public class UpdateCategoryAPITestDataGenerator
    {
        public static IEnumerable<object[]> GetInvalidInputs()
        {
            var fixture = new UpdateCategoryAPITestFixture();
            var invalidInputsList = new List<object[]>();
            var totalInvalidCases = 3;

            for (int index = 0; index < totalInvalidCases; index++)
            {
                switch (index % totalInvalidCases)
                {
                    case 0:
                        UpdateCategoryAPIInput input1 = fixture.GetExampleInput();
                        input1.Name = fixture.GetInvalidNameTooShort();
                        invalidInputsList.Add(
                        [
                            input1,
                            "Name should be at least 3 characters long."
                        ]);
                        break;
                    case 1:
                        UpdateCategoryAPIInput input2 = fixture.GetExampleInput();
                        input2.Name = fixture.GetInvalidNameTooLong();
                        invalidInputsList.Add(
                        [
                            input2,
                            "Name should be less or equal 255 characters long."
                        ]);
                        break;
                    case 2:
                        UpdateCategoryAPIInput input3 = fixture.GetExampleInput();
                        input3.Description = fixture.GetInvalidDescriptionTooLong();
                        invalidInputsList.Add(new object[]
                        {
                                input3,
                                "Description should be less or equal 10000 characters long."
                        });
                        break;
                    default:
                        break;
                }
            }
            return invalidInputsList;
        }
    }
}
