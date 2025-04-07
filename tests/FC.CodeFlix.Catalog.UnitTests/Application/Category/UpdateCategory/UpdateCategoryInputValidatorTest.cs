using FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory;
using FluentAssertions;

namespace FC.CodeFlix.Catalog.UnitTests.Application.Category.UpdateCategory
{
    [Collection(nameof(UpdateCategoryInputValidatorTest))]

    public class UpdateCategoryInputValidatorTest : IClassFixture<UpdateCategoryTestFixture>
    {
        private readonly UpdateCategoryTestFixture _fixture;

        public UpdateCategoryInputValidatorTest(UpdateCategoryTestFixture fixture) => _fixture = fixture;


        [Fact(DisplayName = nameof(DontValidateWhenEmptyGuid))]
        [Trait("Application", "UpdateCategoryInputValidator - UseCases")]
        public void DontValidateWhenEmptyGuid()
        {
            var input = _fixture.GetValidInput(Guid.Empty);
            var validator = new UpdateCategoryInputValidator();
            var validateResult = validator.Validate(input);
            validateResult.Should().NotBeNull();
            validateResult.IsValid.Should().BeFalse();
            validateResult.Errors.Should().HaveCount(1);
            validateResult.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
        }

        [Fact(DisplayName = nameof(ValidateWhenValid))]
        [Trait("Application", "UpdateCategoryInputValidator - UseCases")]
        public void ValidateWhenValid()
        {
            var input = _fixture.GetValidInput();
            var validator = new UpdateCategoryInputValidator();
            var validateResult = validator.Validate(input);
            validateResult.Should().NotBeNull();
            validateResult.IsValid.Should().BeTrue();
            validateResult.Errors.Should().HaveCount(0);
        }
    }
}
