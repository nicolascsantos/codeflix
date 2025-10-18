﻿using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.SeedWork;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;

namespace FC.CodeFlix.Catalog.Domain.Repository
{
    public interface ICastMemberRepository : IGenericRepository<CastMember>, ISearchableRepository<CastMember>
    {
    }
}
