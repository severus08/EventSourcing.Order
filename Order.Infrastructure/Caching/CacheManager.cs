using System;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.Extensions.Options;

namespace Order.Infrastructure.Caching
{
    public class CacheManager : ICacheManager
    {        
        private readonly IEasyCachingProvider _cache;
        public CacheManager(
            IEasyCachingProviderFactory cacheFactory,
            IOptions<CacheAppConfigration> option)
        {
            _cache = cacheFactory.GetCachingProvider(option.Value.InstanceName);
        }

        public async Task<T> GetAsync<T>(string key)
            => Newtonsoft.Json.JsonConvert.DeserializeObject<T>((await _cache.GetAsync<string>(key)).Value);

        public async Task<bool> IsExistsAsync(string key)
            => await _cache.ExistsAsync(key);


        public async Task RemoveAsync(string key)
            => await _cache.RemoveAsync(key);


        public async Task SetAsync(string key, object data, int cacheTime)
            => await _cache.SetAsync(key, (Newtonsoft.Json.JsonConvert.SerializeObject(data ?? throw new ArgumentNullException("data is null"))), TimeSpan.FromHours(cacheTime));
    }
}