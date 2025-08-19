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
    public interface IVillageRepository : IBaseRepository<Village>
    {
        Task<Tuple<List<VillageResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize);
        Task<bool> VillageExistsAsync(string orgName, long id = 0);
        Task<List<DropdownResponseEntity>> GetForDDL();
    }
    public class VillageRepository : BaseRepository<Village>, IVillageRepository
    {
        private readonly am_dbcontext _dbContext;
        public VillageRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<VillageResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _dbContext.Villages.Select(o => new VillageResponseEntity
            {
                id = o.Id,
                vill_name = o.Name,
                district = o.District,
                country = o.Country
            });
            int totalCount = await query.CountAsync();
            var vilalges = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            
            return new Tuple<List<VillageResponseEntity>, int>(vilalges, totalCount);
        }

        public async Task<bool> VillageExistsAsync(string orgName, long id = 0)
        {
            return await _dbContext.Villages.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(orgName.Trim().ToLower()));
        }

        public async Task<List<DropdownResponseEntity>> GetForDDL()
        {
            var villages = await _dbContext.Villages
                .Select(o => new DropdownResponseEntity
                {
                    id = o.Id,
                    name = o.Name
                })
                .ToListAsync();
            return villages;
        }
    }
}
