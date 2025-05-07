using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Interface
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<Tuple<List<UserResponseEntity>, int>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<bool> UserExistsAsync(string email, long id = 0);
        Task<int> DeleteWithUserRole(long userId);
    }
}
