// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>Stores authentication tickets server-side so tokens are not serialized into the browser cookie.</summary>
public sealed class MemoryCacheTicketStore : ITicketStore
{
    private static readonly TimeSpan TicketLifetime = TimeSpan.FromHours(8);
    private readonly IMemoryCache _cache;

    public MemoryCacheTicketStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = $"auth-ticket:{Guid.NewGuid():N}";
        RenewTicket(key, ticket);
        return Task.FromResult(key);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        RenewTicket(key, ticket);
        return Task.CompletedTask;
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key) =>
        Task.FromResult(_cache.Get<AuthenticationTicket>(key));

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    private void RenewTicket(string key, AuthenticationTicket ticket)
    {
        _cache.Set(
            key,
            ticket,
            new MemoryCacheEntryOptions
            {
                SlidingExpiration = TicketLifetime,
                AbsoluteExpirationRelativeToNow = TicketLifetime
            });
    }
}
