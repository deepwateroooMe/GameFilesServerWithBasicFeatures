using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace MyServer {

    public class MemoryCacheTicketStore : ITicketStore {

        private const string KeyPrefix = "AuthSessionStore-";
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpireTimeSpan;

        public MemoryCacheTicketStore(TimeSpan defaultExpireTimeSpan, MemoryCacheOptions options = null) {
            options ??= new MemoryCacheOptions();
            _cache = new MemoryCache(options);
            _defaultExpireTimeSpan = defaultExpireTimeSpan;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket) {
            var guid = Guid.NewGuid();
            var key = KeyPrefix + guid.ToString("N");
            await RenewAsync(key, ticket);
            return key;
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket) {
            var options = new MemoryCacheEntryOptions();
            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue) {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            } else {
                options.SetSlidingExpiration(_defaultExpireTimeSpan);
            }
            _cache.Set(key, ticket, options);
            return Task.CompletedTask;
        }

        public Task<AuthenticationTicket> RetrieveAsync(string key) {
            _cache.TryGetValue(key, out AuthenticationTicket ticket);
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key) {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}