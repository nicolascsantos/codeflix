
namespace FC.CodeFlix.Catalog.API.APIModels.Genre
{
    public class UpdateGenreAPIInput
    {

        public string Name { get; set; }

        public bool? IsActive { get; set; }

        public List<Guid>? CategoriesIds { get; set; }

        public UpdateGenreAPIInput(
            string name,
            bool? isActive = null,
            List<Guid>? categoriesIds = null
        )
        {
            Name = name;
            IsActive = isActive;
            CategoriesIds = categoriesIds;
        }
    }
}
