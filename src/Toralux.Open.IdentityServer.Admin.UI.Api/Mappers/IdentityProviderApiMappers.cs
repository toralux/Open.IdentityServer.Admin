// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.IdentityProvider;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    internal static partial class IdentityProviderApiMapper
    {
        [MapProperty(nameof(IdentityProviderDto.Properties), nameof(IdentityProviderApiDto.IdentityProviderProperties))]
        public static partial IdentityProviderApiDto ToIdentityProviderApiDto(IdentityProviderDto source);

        [MapProperty(nameof(IdentityProviderApiDto.IdentityProviderProperties), nameof(IdentityProviderDto.Properties))]
        public static partial IdentityProviderDto ToIdentityProviderDto(IdentityProviderApiDto source);

        public static partial IdentityProvidersApiDto ToIdentityProvidersApiDto(IdentityProvidersDto source);
        public static partial IdentityProvidersDto ToIdentityProvidersDto(IdentityProvidersApiDto source);

        private static Dictionary<string, string> MapProperties(Dictionary<int, IdentityProviderPropertyDto> source)
        {
            if (source == null || source.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            return source
                .Where(x => !string.IsNullOrWhiteSpace(x.Value?.Name))
                .Select(x => x.Value)
                // Duplicate names are resolved with last-write-wins to match dictionary-like update semantics.
                .GroupBy(x => x.Name, System.StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.Last().Value, System.StringComparer.Ordinal);
        }

        private static Dictionary<int, IdentityProviderPropertyDto> MapProperties(Dictionary<string, string> source)
        {
            if (source == null)
            {
                return new Dictionary<int, IdentityProviderPropertyDto>();
            }

            var index = 0;
            var values = source.Select(item => new IdentityProviderPropertyDto { Name = item.Key, Value = item.Value });
            return values.ToDictionary(_ => index++, item => item);
        }
    }

    public static class IdentityProviderApiMappers
    {
        public static IdentityProviderApiDto ToIdentityProviderApiDto(this IdentityProviderDto source) => IdentityProviderApiMapper.ToIdentityProviderApiDto(source);
        public static IdentityProviderDto ToIdentityProviderDto(this IdentityProviderApiDto source) => IdentityProviderApiMapper.ToIdentityProviderDto(source);

        public static IdentityProvidersApiDto ToIdentityProvidersApiDto(this IdentityProvidersDto source) => IdentityProviderApiMapper.ToIdentityProvidersApiDto(source);
        public static IdentityProvidersDto ToIdentityProvidersDto(this IdentityProvidersApiDto source) => IdentityProviderApiMapper.ToIdentityProvidersDto(source);
    }
}
