// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Entities;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class PersistedGrantMapper
    {
        public static partial PersistedGrantDto ToPersistedGrantDto(PersistedGrant source);
        public static partial PersistedGrantDto ToPersistedGrantDto(PersistedGrantDataView source);
    }

    public static class PersistedGrantMappers
    {
        public static PersistedGrantsDto ToModel(this PagedList<PersistedGrantDataView> grant)
        {
            if (grant == null) return null;

            return new PersistedGrantsDto
            {
                TotalCount = grant.TotalCount,
                PageSize = grant.PageSize,
                PersistedGrants = grant.Data.Select(PersistedGrantMapper.ToPersistedGrantDto).ToList()
            };
        }

        public static PersistedGrantsDto ToModel(this PagedList<PersistedGrant> grant)
        {
            if (grant == null) return null;

            return new PersistedGrantsDto
            {
                TotalCount = grant.TotalCount,
                PageSize = grant.PageSize,
                PersistedGrants = grant.Data.Select(PersistedGrantMapper.ToPersistedGrantDto).ToList()
            };
        }

        public static PersistedGrantDto ToModel(this PersistedGrant grant)
        {
            return grant == null ? null : PersistedGrantMapper.ToPersistedGrantDto(grant);
        }
    }
}
