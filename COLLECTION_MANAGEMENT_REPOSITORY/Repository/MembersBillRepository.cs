using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public interface IMembersBillRepository : IBaseRepository<MembersBill>
    {
        Task<Tuple<List<MembersBillResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize);
        Task<bool> BillExistsAsync(long memberId, DateTime fromDate, DateTime toDate, long id = 0);
    }

    public class MembersBillRepository : BaseRepository<MembersBill>, IMembersBillRepository
    {
        private readonly am_dbcontext _dbContext;
        public MembersBillRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<MembersBillResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = from mb in _dbContext.MembersBills
                        join m in _dbContext.Members on mb.MemberId equals m.Id
                        where mb.Status == (int)CommonEnum.Status.ACTIVE
                        select new
                        {
                            id = mb.Id,
                            member_name = m.Name,
                            amount = mb.Amount,
                            from_date = mb.FromDate.ToString("dd MMM, yyyy"),
                            to_date = mb.FromDate.ToString("dd MMM, yyyy"),
                            status = m.Status.ToString()
                        };
            int totalCount = await query.CountAsync();
            var orgs = await query.OrderByDescending(d => d.id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = orgs.Select(o => new MembersBillResponseEntity
            {
                id = o.id,
                member_name = o.member_name,
                amount = o.amount,
                from_date = o.from_date,
                to_date = o.to_date,
                status = !string.IsNullOrWhiteSpace(o.status) ? Enum.GetName(typeof(CommonEnum.Status), Convert.ToInt16(o.status)) : string.Empty
            }).ToList();
            return new Tuple<List<MembersBillResponseEntity>, int>(result, totalCount);
        }

        public async Task<bool> BillExistsAsync(long memberId, DateTime fromDate, DateTime toDate, long id = 0)
        {
            return await _dbContext.MembersBills.AnyAsync(d =>
                    (id != 0 ? d.Id != id : true) &&
                    d.MemberId == memberId &&
                    d.Status == (int)CommonEnum.Status.ACTIVE &&
                    fromDate <= d.ToDate &&
                    toDate >= d.FromDate
                );
        }
    }
}
