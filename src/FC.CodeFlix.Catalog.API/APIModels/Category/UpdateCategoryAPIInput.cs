namespace FC.CodeFlix.Catalog.API.APIModels.Category
{
    public class UpdateCategoryAPIInput
    {

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
        public UpdateCategoryAPIInput(string name, string? description = null, bool? isActive = null)
        {
            Name = name;
            Description = description;
            IsActive = isActive;
        }
    }
}
