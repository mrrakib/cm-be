using COLLECTION_MANAGEMENT_ENTITIES.RequestEntity;
using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface IUserManager
    {
        Task<CommonResponse> GetAllAsync();
        Task<CommonResponse> GetAllPagedAsync(int page, int pageSize);
        Task<CommonResponse> GetByIdAsync(long id);
        Task<CommonResponse> UpdateAsync(UserRequestEntity userRequestEntity);
        Task<CommonResponse> DeleteAsync(long id);
        Task<CommonResponse> GetAllGenderAsync();
    }
}
