using FluentValidation;

namespace FC.CodeFlix.Catalog.Application.UseCases.Category.GetCategory
{
    public class GetCategoryInputValidator : AbstractValidator<GetCategoryInput>
    {
        public GetCategoryInputValidator() => RuleFor(i => i.Id).NotEmpty().WithMessage("'Id' must not be empty.");
    }
}
