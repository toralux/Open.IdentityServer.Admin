// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Threading;
using System.Threading.Tasks;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces
{
    public interface IKeyService
    {
        Task<KeysDto> GetKeysAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<KeyDto> GetKeyAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> ExistsKeyAsync(string id, CancellationToken cancellationToken = default);
        Task DeleteKeyAsync(string id, CancellationToken cancellationToken = default);
    }
}