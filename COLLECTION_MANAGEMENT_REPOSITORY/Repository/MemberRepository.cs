using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.EntityFrameworkCore;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public interface IMemberRepository : IBaseRepository<Member>
    {
        Task<Tuple<List<MemberResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize);
        Task<bool> OrgExistsAsync(string orgName, long id = 0);
        Task<List<DropdownResponseEntity>> GetForDDL();
    }
    public class MemberRepository : BaseRepository<Member>, IMemberRepository
    {
        private readonly am_dbcontext _dbContext;
        public MemberRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<MemberResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = from m in _dbContext.Members
                        join v in _dbContext.Villages on m.VillageId equals v.Id into villageGroup
                        from v in villageGroup.DefaultIfEmpty()
                        select new
                        {
                            id = m.Id,
                            member_name = m.Name,
                            contact_no = m.ContactNo,
                            email = m.Email,
                            address = m.Address,
                            village_id = m.VillageId,
                            village_name = v != null ? v.Name : string.Empty,
                            status = m.Status.ToString()
                        };
            int totalCount = await query.CountAsync();
            var orgs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = orgs.Select(o => new MemberResponseEntity
            {
                id = o.id,
                member_name = o.member_name,
                contact_no = o.contact_no,
                email = o.email,
                address = o.address,
                village_id = o.village_id,
                village_name = o.village_name,
                status = !string.IsNullOrWhiteSpace(o.status) ? Enum.GetName(typeof(CommonEnum.Status), Convert.ToInt16(o.status)) : string.Empty
            }).ToList();
            return new Tuple<List<MemberResponseEntity>, int>(result, totalCount);
        }

        public async Task<bool> OrgExistsAsync(string orgName, long id = 0)
        {
            return await _dbContext.Villages.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(orgName.Trim().ToLower()));
        }

        public async Task<List<DropdownResponseEntity>> GetForDDL()
        {
            var members = await _dbContext.Villages
                .Select(o => new DropdownResponseEntity
                {
                    id = o.Id,
                    name = o.Name
                })
                .ToListAsync();
            return members;
        }
    }
}
