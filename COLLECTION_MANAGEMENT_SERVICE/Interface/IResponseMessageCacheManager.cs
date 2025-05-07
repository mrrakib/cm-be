using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Interface
{
    public interface IResponseMessageCacheManager
    {
        void AddToCache(List<ResponseMessage> responseMessages);
        Task<List<ResponseMessage>> GetCachedResponseMessage();
    }
}
