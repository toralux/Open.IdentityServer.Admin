// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Key;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    internal static partial class KeyApiMapper
    {
        public static partial KeyApiDto ToKeyApiDto(KeyDto source);
        public static partial KeyDto ToKeyDto(KeyApiDto source);

        public static partial KeysApiDto ToKeysApiDto(KeysDto source);
        public static partial KeysDto ToKeysDto(KeysApiDto source);
    }

    public static class KeyApiMappers
    {
        public static KeyApiDto ToKeyApiDto(this KeyDto source) => KeyApiMapper.ToKeyApiDto(source);
        public static KeyDto ToKeyDto(this KeyApiDto source) => KeyApiMapper.ToKeyDto(source);

        public static KeysApiDto ToKeysApiDto(this KeysDto source) => KeyApiMapper.ToKeysApiDto(source);
        public static KeysDto ToKeysDto(this KeysApiDto source) => KeyApiMapper.ToKeysDto(source);
    }
}
