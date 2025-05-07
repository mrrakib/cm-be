using COLLECTION_MANAGEMENT_ENTITIES.ResponseEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface ICommonManager
    {
        Task<CommonResponse> HandleResponse(int httpStatusCodeEnum, int errorCode, CommonResponse responseEntity, string tag = "", string custom_message = "", int? custom_code = null);
    }
}
