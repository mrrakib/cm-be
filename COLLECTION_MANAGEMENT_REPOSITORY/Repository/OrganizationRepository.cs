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
    public interface IOrganizationRepository : IBaseRepository<Organization>
    {
        Task<Tuple<List<OrganizationResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize);
        Task<bool> OrgExistsAsync(string orgName, long id = 0);
        Task<List<DropdownResponseEntity>> GetForDDL();
    }
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        private readonly am_dbcontext _dbContext;
        public OrganizationRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<OrganizationResponseEntity>, int>> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _dbContext.Organizations.Select(o => new OrganizationResponseEntity
            {
                id = o.Id,
                org_name = o.Name,
                mobile_no = o.MobileNo,
                email = o.Email,
                address = o.Address,
                status = o.Status.ToString()
            });
            int totalCount = await query.CountAsync();
            var orgs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var result = orgs.Select(o => new OrganizationResponseEntity
            {
                id = o.id,
                org_name = o.org_name,
                mobile_no = o.mobile_no,
                email = o.email,
                address = o.address,
                status = !string.IsNullOrWhiteSpace(o.status) ? Enum.GetName(typeof(CommonEnum.Status), Convert.ToInt16(o.status)) : string.Empty
            }).ToList();
            return new Tuple<List<OrganizationResponseEntity>, int>(result, totalCount);
        }

        public async Task<bool> OrgExistsAsync(string orgName, long id = 0)
        {
            return await _dbContext.Organizations.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(orgName.Trim().ToLower()));
        }

        public async Task<List<DropdownResponseEntity>> GetForDDL()
        {
            var organizations = await _dbContext.Organizations
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
