using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = FC.CodeFlix.Catalog.Domain.Entity;

namespace FC.CodeFlix.Catalog.EndToEndTests.API.CastMember.Common
{
    public class CastMemberPersistence
    {
        private readonly CodeflixCatalogDbContext _context;

        public CastMemberPersistence(CodeflixCatalogDbContext context)
            => _context = context;

        public async Task InsertList(List<DomainEntity.CastMember> castMembers)
        {
            await _context.CastMembers.AddRangeAsync(castMembers);
            var test = await _context.SaveChangesAsync();
        }

        public async Task<DomainEntity.CastMember?> GetById(Guid id)
            => await _context.CastMembers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }
}
