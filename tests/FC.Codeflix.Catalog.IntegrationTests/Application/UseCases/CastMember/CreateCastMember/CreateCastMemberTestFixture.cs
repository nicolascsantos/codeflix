using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.Common;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.CastMember.CreateCastMember
{
    [CollectionDefinition(nameof(CreateCastMemberTestFixture))]
    public class CreateCastMemberTestFixtureCollection : ICollectionFixture<CreateCastMemberTestFixture> { }

    public class CreateCastMemberTestFixture : CastMemberUseCasesTestFixture
    {
    }
}
