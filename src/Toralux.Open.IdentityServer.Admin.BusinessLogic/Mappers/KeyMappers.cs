// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    internal static partial class KeyMapper
    {
        public static partial KeyDto ToKeyDto(Key source);
    }

    public static class KeyMappers
    {
        public static KeyDto ToModel(this Key key)
        {
            return key == null ? null : KeyMapper.ToKeyDto(key);
        }

        public static KeysDto ToModel(this PagedList<Key> grant)
        {
            if (grant == null) return null;

            return new KeysDto
            {
                TotalCount = grant.TotalCount,
                PageSize = grant.PageSize,
                Keys = grant.Data.Select(KeyMapper.ToKeyDto).ToList()
            };
        }
    }
}
