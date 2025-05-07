using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_REPOSITORY.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private readonly am_dbcontext _dbContext;

        public CommonRepository(am_dbcontext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<ResponseMessage>> GetResponseMessages()
        {
            return await _dbContext.ResponseMessages.ToListAsync();
        }
    }
}
