using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Application.Interfaces;
using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using FC.CodeFlix.Catalog.Domain.Repository;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.UpdateCastMember
{
    public class UpdateCastMember : IUpdateCastMember
    {
        private readonly ICastMemberRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCastMember(ICastMemberRepository repository, IUnitOfWork unitOfWork)
            => (_repository, _unitOfWork) = (repository, unitOfWork);
        

        public async Task<CastMemberModelOutput> Handle(UpdateCastMemberInput request, CancellationToken cancellationToken)
        {
            var castMember = await _repository.Get(request.Id, cancellationToken);
            NotFoundException.ThrowIfNull(castMember, $"Cast member '{request.Id}' not found.");
            castMember.Update(request.Name, request.Type);
            await _repository.Update(castMember, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            return CastMemberModelOutput.FromCastMember(castMember);
        }
    }
}
