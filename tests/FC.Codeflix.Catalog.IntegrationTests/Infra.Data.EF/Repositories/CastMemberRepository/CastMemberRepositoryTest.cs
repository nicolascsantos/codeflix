﻿using FC.Codeflix.Catalog.Infra.Data.EF;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Repository = FC.Codeflix.Catalog.Infra.Data.EF.Repositories;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CastMemberRepository
{
    [Collection(nameof(CastMemberRepositoryTestFixture))]
    public class CastMemberRepositoryTest
    {
        private readonly CastMemberRepositoryTestFixture _fixture;

        public CastMemberRepositoryTest(CastMemberRepositoryTestFixture fixture)
            => _fixture = fixture;

        [Fact(DisplayName = nameof(Insert))]
        [Trait("Integration/Infra.Data", "CastMemberRepository - Repositories")]
        public async Task Insert()  
        {
            var castMemberExample = _fixture.GetExampleCastMember();
            CodeflixCatalogDbContext context = _fixture.CreateDbContext();
            var repository = new Repository.CastMemberRepository(context);

            await repository.Insert(castMemberExample, CancellationToken.None);
            await context.SaveChangesAsync();

            var assertionContext = _fixture.CreateDbContext(true);
            var castMemberFromDb = await assertionContext.CastMembers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == castMemberExample.Id);

            castMemberFromDb.Should().NotBeNull();
            castMemberFromDb.Name.Should().Be(castMemberExample.Name);
            castMemberFromDb.Type.Should().Be(castMemberExample.Type);
        }
    }
}
