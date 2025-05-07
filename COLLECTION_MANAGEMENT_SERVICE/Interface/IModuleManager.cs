using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface IModuleManager
    {
        Task<CommonResponse> GetAllAsync();
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> CreateAsync(ModuleRequestEntity moduleRequestEntity);
        Task<CommonResponse> UpdateAsync(ModuleRequestEntity moduleRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
    }
}
