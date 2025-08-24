using FC.CodeFlix.Catalog.Domain.Enum;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.Validation;

namespace FC.CodeFlix.Catalog.Domain.Entity
{
    public class CastMember : AggregateRoot
    {
        public string Name { get; set; }

        public CastMemberType Type { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public CastMember(string name, CastMemberType type)
        {
            Name = name;
            Type = type;
            CreatedAt = DateTime.Now;
            Validate();
        }

        public void Validate()
        {
            DomainValidation.NotNullOrEmpty(Name, nameof(Name));
        }

        public void Update(string name, CastMemberType type)
        {
            Name = name;
            Type = type;
            Validate();
        }
    }
}
