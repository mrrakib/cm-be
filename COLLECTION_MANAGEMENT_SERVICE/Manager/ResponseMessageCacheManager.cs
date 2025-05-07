using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Models;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COLLECTION_MANAGEMENT_SERVICE.Manager
{
    public class ResponseMessageCacheManager : IResponseMessageCacheManager
    {
        private const string ResponseMessageCacheKey = "response_message_cache";
        private readonly IMemoryCache _memoryCache;
        private readonly ICommonRepository _commonRepository;

        public ResponseMessageCacheManager(IMemoryCache memoryCache, ICommonRepository commonRepository)
        {
            _memoryCache = memoryCache;
            _commonRepository = commonRepository;
        }

        public void AddToCache(List<ResponseMessage> responseMessages)
        {
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(12),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            _memoryCache.Set(ResponseMessageCacheKey, responseMessages, cacheEntryOptions);
        }

        public async Task<List<ResponseMessage>> GetCachedResponseMessage()
        {

            if (_memoryCache.TryGetValue(ResponseMessageCacheKey, out List<ResponseMessage> responseMessages_cache))
            {
                return _memoryCache.Get<List<ResponseMessage>>(ResponseMessageCacheKey);
            }
            List<ResponseMessage> responseMessages = await _commonRepository.GetResponseMessages();
            AddToCache(responseMessages);
            return responseMessages;
        }
    }
}
