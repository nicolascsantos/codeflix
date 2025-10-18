using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.GetCastMember
{
    public class GetCastMember : IGetCastMember
    {
        private readonly ICastMemberRepository _repository;

        public GetCastMember(ICastMemberRepository repository)
            => (_repository) = (repository);

        public async Task<CastMemberModelOutput> Handle(GetCastMemberInput request, CancellationToken cancellationToken)
        {
            var castMember = await _repository.Get(request.Id, cancellationToken);
            return CastMemberModelOutput.FromCastMember(castMember);
        }
    }
}
