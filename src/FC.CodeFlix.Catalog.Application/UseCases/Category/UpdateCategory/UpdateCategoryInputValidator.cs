using FluentValidation;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.UpdateCategory
{
    public class UpdateCategoryInputValidator : AbstractValidator<UpdateCategoryInput>
    {
        public UpdateCategoryInputValidator()
        {
            RuleFor(i => i.Id).NotEmpty().WithMessage("'Id' must not be empty.");
        }
    }
}
