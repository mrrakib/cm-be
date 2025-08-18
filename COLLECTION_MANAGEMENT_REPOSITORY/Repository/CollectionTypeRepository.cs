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
    public interface ICollectionTypeRepository : IBaseRepository<CollectionType>
    {
        Task<Tuple<List<CollectionTypeResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize);
        Task<bool> TypeExistsAsync(string finYearName, long id = 0);
        Task<List<DropdownResponseEntity>> GetForDDL();
    }
    public class CollectionTypeRepository : BaseRepository<CollectionType>, ICollectionTypeRepository
    {
        private readonly am_dbcontext _dbContext;
        public CollectionTypeRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<CollectionTypeResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _dbContext.CollectionTypes.Select(o => new CollectionTypeResponseEntity
            {
                id = o.Id,
                type_name = o.Name,
                description = o.Description,
                is_monthly = o.IsMonthly
            });
            int totalCount = await query.CountAsync();
            var orgs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = orgs.Select(o => new CollectionTypeResponseEntity
            {
                id = o.id,
                type_name = o.type_name,
                description = o.description,
                is_monthly = o.is_monthly
            }).ToList();
            return new Tuple<List<CollectionTypeResponseEntity>, int>(result, totalCount);
        }

        public async Task<bool> TypeExistsAsync(string finYearName, long id = 0)
        {
            return await _dbContext.CollectionTypes.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(finYearName.Trim().ToLower()));
        }

        public async Task<List<DropdownResponseEntity>> GetForDDL()
        {
            var types = await _dbContext.CollectionTypes
                .Select(o => new DropdownResponseEntity
                {
                    id = o.Id,
                    name = o.Name
                })
                .ToListAsync();
            return types;
        }
    }
}
