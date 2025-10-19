using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.ListCastMembers
{
    public class ListCastMembers : IListCastMembers
    {
        private readonly ICastMemberRepository _repository;

        public ListCastMembers(ICastMemberRepository repository)
            => _repository = repository;

        public async Task<ListCastMembersOutput> Handle(ListCastMembersInput request, CancellationToken cancellationToken)
        {
            var searchOutput = await _repository.Search(request.ToSearchInput(), cancellationToken);
            return ListCastMembersOutput.FromSearchOutput(searchOutput);
        }
    }
}
