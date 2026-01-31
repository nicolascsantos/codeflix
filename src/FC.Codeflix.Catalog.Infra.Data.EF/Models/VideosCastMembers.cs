using FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Models
{
    public class VideosCastMembers
    {
        public Guid VideoId { get; private set; }

        public Guid CastMemberId { get; private set; }

        public Video? Video { get; set; }

        public CastMember? CastMember { get; set; }

        public VideosCastMembers(Guid videoId, Guid castMemberId)
        {
            VideoId = videoId;
            CastMemberId = castMemberId;
        }
    }
}
