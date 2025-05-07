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
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly identity_dbcontext _dbContext;

        public UserRepository(identity_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Tuple<List<UserResponseEntity>, int>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            //var query = from u in _dbContext.Users
            //            join ur in _dbContext.UserRoles on u.Id equals ur.UserId into userRoles
            //            from userRole in userRoles.DefaultIfEmpty()
            //            join r in _dbContext.Roles on 
            //            select new UserResponseEntity
            //            {
            //                id = u.Id,
            //                user_name = u.UserName,
            //                full_name = u.FullName,
            //                email = u.Email,
            //                gender = u.Gender.ToString(),
            //                dob = u.DateOfBirth != null ? u.DateOfBirth.Value.ToString("yyyy-dd-MM") : string.Empty
            //            };
            var query = _dbContext.Users
                    .GroupJoin(_dbContext.UserRoles,
                        user => user.Id,
                        userRole => userRole.UserId,
                        (user, userRoles) => new { user, userRoles })
                    .SelectMany(
                        temp => temp.userRoles.DefaultIfEmpty(),
                        (temp, userRole) => new { temp.user, userRole })
                    .GroupJoin(_dbContext.Roles,
                        temp => temp.userRole != null ? temp.userRole.RoleId : (long?)null,
                        role => role.Id,
                        (temp, roles) => new { temp.user, temp.userRole, roles })
                    .SelectMany(
                        temp => temp.roles.DefaultIfEmpty(),
                        (temp, role) => new UserResponseEntity
                        {
                            id = temp.user.Id,
                            user_name = temp.user.UserName,
                            full_name = temp.user.FullName,
                            email = temp.user.Email,
                            contact_no = temp.user.ContactNo,
                            role = role != null ? role.Name : string.Empty
                        });
            int totalCount = await query.CountAsync();
            var users = await query
                                .OrderBy(d => d.email)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize).ToListAsync();
            users = users.Select(u => new UserResponseEntity
            {
                id = u.id,
                user_name = u.user_name,
                full_name = u.full_name,
                email = u.email,
                gender = u.gender,
                gender_name = !string.IsNullOrWhiteSpace(u.gender_name) ? Enum.GetName(typeof(CommonEnum.Gender), int.Parse(u.gender_name)) : string.Empty,
                birth_date = u.birth_date
            }).ToList();
            return new Tuple<List<UserResponseEntity>, int>(users, totalCount);
        }

        public async Task<bool> UserExistsAsync(string email, long id = 0)
        {
            return await _dbContext.Users.AnyAsync(d => (id != 0 ? d.Id != id : true) && !string.IsNullOrWhiteSpace(d.Email) && d.Email.ToLower().Equals(email.Trim().ToLower()));
        }

        public async Task<int> DeleteWithUserRole(long userId)
        {
            var userRole = await _dbContext.UserRoles.Where(d => d.UserId == userId).FirstOrDefaultAsync();
            if (userRole != null)
                _dbContext.UserRoles.Remove(userRole);

            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
                _dbContext.Users.Remove(user);

            return await _dbContext.SaveChangesAsync();
        }

    }
}
