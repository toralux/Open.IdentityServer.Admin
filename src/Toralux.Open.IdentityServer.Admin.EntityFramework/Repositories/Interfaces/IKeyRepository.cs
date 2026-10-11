// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading;
using System.Threading.Tasks;
using Open.IdentityServer.EntityFramework.Entities;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces
{
    public interface IKeyRepository
    {
        Task<PagedList<Key>> GetKeysAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<Key> GetKeyAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsKeyAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteKeyAsync(string id, CancellationToken cancellationToken = default);
        Task<int> SaveAllChangesAsync(CancellationToken cancellationToken = default);
        bool AutoSaveChanges { get; set; }
    }
}