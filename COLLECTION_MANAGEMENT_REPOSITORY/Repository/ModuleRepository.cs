using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public class ModuleRepository : BaseRepository<Module>, IModuleRepository
    {
        private readonly am_dbcontext _dbContext;

        public ModuleRepository(am_dbcontext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ModuleExistsAsync(string moduleName, long id = 0)
        {
            return await _dbContext.Modules.AnyAsync(d => !string.IsNullOrWhiteSpace(d.Name) && (id != 0 ? d.Id != id : true) && d.Name.ToLower().Equals(moduleName.Trim().ToLower()));
        }

        public async Task<bool> FindDependancyAsync(long id)
        {
            return await _dbContext.Menus.AnyAsync(d => d.ModuleId == id);
        }
    }
}
