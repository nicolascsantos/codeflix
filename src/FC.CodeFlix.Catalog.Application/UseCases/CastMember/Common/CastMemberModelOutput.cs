﻿using FC.CodeFlix.Catalog.Domain.Enum;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common
{
    public class CastMemberModelOutput
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public CastMemberType Type { get; private set; }

        public DateTime CreatedAt { get; set; }

        public CastMemberModelOutput(Guid id, string name, CastMemberType type, DateTime createdAt)
        {
            Id = id;
            Name = name;
            Type = type;
            CreatedAt = createdAt;
        }

        public static CastMemberModelOutput FromCastMember(DomainEntity.CastMember castMember)
            => new CastMemberModelOutput(castMember.Id, castMember.Name, castMember.Type, castMember.CreatedAt);
    }
}
