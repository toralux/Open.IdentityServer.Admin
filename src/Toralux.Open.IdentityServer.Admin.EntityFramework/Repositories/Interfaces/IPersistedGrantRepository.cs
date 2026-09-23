// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Open.IdentityServer.EntityFramework.Entities;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Entities;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces
{
	public interface IPersistedGrantRepository
    {
		Task<PagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10);
		Task<PagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10);
	    Task<PersistedGrant> GetPersistedGrantAsync(string key);
	    Task<int> DeletePersistedGrantAsync(string key);
	    Task<int> DeletePersistedGrantsAsync(string userId);
        Task<bool> ExistsPersistedGrantsAsync(string subjectId);
	    Task<int> SaveAllChangesAsync();
        bool AutoSaveChanges { get; set; }
    }
}