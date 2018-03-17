// Copyright (c) Pawel Kadluczka, Inc. All rights reserved. See License.txt in the project root for license information.

// Created based on the ICache.cs from Tracing and Caching Provider Wrappers for Entity Framework sample to make it easy to port exisiting applications

using System.Threading;
using System.Threading.Tasks;

namespace EntityFramework.Caching.Abstractions
{
    /// <summary>
    /// Interface to be implemented by cache implementations.
    /// </summary>
    public interface ICache
    {
        ICacheOptions Options { get; }
        byte[] Get(string key);

        Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken));

        void Set(string key, byte[] value, DistributedCacheEntryOptions options= default(DistributedCacheEntryOptions));

        Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions), CancellationToken token = default(CancellationToken));

        void SetIdArray(string key, byte[][] value, DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions));

        Task SetIdArrayAsync(string key, byte[][] value, DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions), CancellationToken token = default(CancellationToken));
        byte[][] GetIdArray(string key);
        Task<byte[][]> GetIdArrayAsync(string key, CancellationToken token = default(CancellationToken));

        void Refresh(string key);

        Task RefreshAsync(string key, CancellationToken token = default(CancellationToken));

        void Remove(string key);

        Task RemoveAsync(string key, CancellationToken token = default(CancellationToken));

        void SetDependency(string setKey, string dependantKey, DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions));
        Task SetDependencyAsync(string setKey, string dependantKey, DistributedCacheEntryOptions options = default(DistributedCacheEntryOptions), CancellationToken token = default(CancellationToken));

        void ExpireDependendants(string setKey);
        Task ExpireDependendantsAsync(string setKey, CancellationToken token = default(CancellationToken));
    }
}
