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
using static COLLECTION_MANAGEMENT_UTILITY.CommonEnum;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public interface IFinancialYearRepository : IBaseRepository<FinancialYear>
    {
        Task<Tuple<List<FinYearResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize);
        Task<bool> FinYearExistsAsync(string finYearName, long id = 0);
        Task<List<DropdownResponseEntity>> GetForDDL();
    }
    public class FinancialYearRepository : BaseRepository<FinancialYear>, IFinancialYearRepository
    {
        private readonly am_dbcontext _dbContext;
        public FinancialYearRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<FinYearResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _dbContext.FinancialYears.Select(o => new FinYearResponseEntity
            {
                id = o.Id,
                fin_name = o.Name,
                from_date = o.FromDate.ToString("dd MMM,yyyy"),
                to_date = o.ToDate.ToString("dd MMM,yyyy"),
                status = o.Status.ToString()
            });
            int totalCount = await query.CountAsync();
            var orgs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = orgs.Select(o => new FinYearResponseEntity
            {
                id = o.id,
                fin_name = o.fin_name,
                from_date = o.from_date,
                to_date = o.to_date,
                status = !string.IsNullOrWhiteSpace(o.status) ? Enum.GetName(typeof(CommonEnum.Status), Convert.ToInt16(o.status)) : string.Empty
            }).ToList();
            return new Tuple<List<FinYearResponseEntity>, int>(result, totalCount);
        }

        public async Task<bool> FinYearExistsAsync(string finYearName, long id = 0)
        {
            return await _dbContext.FinancialYears.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(finYearName.Trim().ToLower()));
        }

        public async Task<List<DropdownResponseEntity>> GetForDDL()
        {
            var organizations = await _dbContext.FinancialYears
                .Select(o => new DropdownResponseEntity
                {
                    id = o.Id,
                    name = o.Name
                })
                .ToListAsync();
            return organizations;
        }
    }
}
