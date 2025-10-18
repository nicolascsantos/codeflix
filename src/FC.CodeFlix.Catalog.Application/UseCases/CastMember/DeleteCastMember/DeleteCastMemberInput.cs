using FC.CodeFlix.Catalog.Application.UseCases.CastMember.Common;
using MediatR;

namespace FC.CodeFlix.Catalog.Application.UseCases.CastMember.DeleteCastMember
{
    public class DeleteCastMemberInput : IRequest<Unit>
    {
        public Guid Id { get; set; }

        public DeleteCastMemberInput(Guid id)
            => Id = Id;
    }
}
