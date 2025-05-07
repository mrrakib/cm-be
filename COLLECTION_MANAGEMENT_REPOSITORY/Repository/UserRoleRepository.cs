using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_UTILITY;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public class UserRoleRepository : BaseRepository<IdentityUserRole<long>>, IUserRoleRepository
    {
        private readonly identity_dbcontext _dbContext;

        public UserRoleRepository(identity_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<UserRoleResponseEntity>, int>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var query = from ur in _dbContext.UserRoles
                        join u in _dbContext.Users on ur.UserId equals u.Id
                        join r in _dbContext.Roles on ur.RoleId equals r.Id
                        select new UserRoleResponseEntity
                        {
                            user_id = u.Id,
                            user_name = u.Email,
                            role_id = r.Id,
                            role_name = r.Name,
                        };
            int totalCount = await query.CountAsync();
            var userRoles = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new Tuple<List<UserRoleResponseEntity>, int>(userRoles, totalCount);
        }

        public async Task<UserRoleResponseEntity?> GetUserRoleByUserId(long userId)
        {
            var query = from ur in _dbContext.UserRoles
                        join u in _dbContext.Users on ur.UserId equals u.Id
                        join r in _dbContext.Roles on ur.RoleId equals r.Id
                        where u.Id == userId
                        select new UserRoleResponseEntity
                        {
                            user_id = u.Id,
                            user_name = u.Email,
                            role_id = r.Id,
                            role_name = r.Name,
                        };
            return await query.FirstOrDefaultAsync();
        }

        public async Task UpdateUserRole(UserRoleRequestEntity userRoleRequestEntity)
        {
            IdentityUserRole<long>? userRole = await _dbContext.UserRoles.Where(d => d.UserId == userRoleRequestEntity.user_id).FirstOrDefaultAsync();
            if (userRole == null)
            {
                return;
            }
            _dbContext.UserRoles.Remove(userRole);
            await _dbContext.SaveChangesAsync();

            IdentityUserRole<long>? newUserRole = new IdentityUserRole<long>
            {
                UserId = userRoleRequestEntity.user_id,
                RoleId = userRoleRequestEntity.role_id,
            };
            await _dbContext.UserRoles.AddAsync(newUserRole);
        }

        public async Task<bool> UserRoleExistsAsync(long user_id, long role_id = 0)
        {
            return await _dbContext.UserRoles.AnyAsync(d => d.UserId == user_id && ( role_id == 0 ? true : d.RoleId == role_id));
        }

    }
}
