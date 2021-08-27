using Dangl.Identity.Client.Mvc.Services;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Dangl.OpenCDE.TestUtilities
{
    public class MockUserInfoUpdaterCache : IUserInfoUpdaterCache
    {
        private readonly ConcurrentDictionary<Guid, DateTime> _internalCache = new ConcurrentDictionary<Guid, DateTime>();

        public Task<bool> HasUserIdCachedAsync(Guid userId)
        {
            return Task.FromResult(_internalCache.TryGetValue(userId, out var expiration) && expiration >= DateTime.UtcNow);
        }

        public Task CacheUserIdAsync(Guid userId, DateTime expiration)
        {
            _internalCache.TryAdd(userId, expiration);
            return Task.CompletedTask;
        }

        public void Clear()
        {
            _internalCache.Clear();
        }
    }
}
