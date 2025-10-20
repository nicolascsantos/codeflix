using FC.CodeFlix.Catalog.Domain.Enum;

namespace FC.CodeFlix.Catalog.API.APIModels.CastMember
{
    public class UpdateCastMemberAPIInput
    {
        public string Name { get; set; }

        public CastMemberType Type { get; set; }

        public UpdateCastMemberAPIInput(string name, CastMemberType type)
        {
            Name = name;
            Type = type;
        }
    }
}
