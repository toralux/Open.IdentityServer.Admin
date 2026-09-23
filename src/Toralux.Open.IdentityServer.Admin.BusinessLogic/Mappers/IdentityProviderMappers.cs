// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class IdentityProviderMapper
    {
        public static partial IdentityProviderDto ToIdentityProviderDto(IdentityProvider source);
        public static partial IdentityProvider ToIdentityProvider(IdentityProviderDto source);

        private static string MapProperties(Dictionary<int, IdentityProviderPropertyDto> source)
        {
            if (source == null || source.Count == 0)
            {
                return "{}";
            }

            var dict = source
                .Where(x => x.Value != null && x.Value.Name != null)
                .ToDictionary(x => x.Value.Name, x => x.Value.Value);
            return JsonSerializer.Serialize(dict);
        }

        private static Dictionary<int, IdentityProviderPropertyDto> MapProperties(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return [];
            }

            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(source);
                if (dict == null || dict.Count == 0)
                {
                    return [];
                }

                var index = 0;
                return dict.ToDictionary(_ => index++, item => new IdentityProviderPropertyDto { Name = item.Key, Value = item.Value });
            }
            catch (JsonException)
            {
                // Be resilient against malformed persisted values and keep read-side behavior tolerant.
                return [];
            }
        }
    }

    public static class IdentityProviderMappers
    {
        public static IdentityProviderDto ToModel(this IdentityProvider identityProvider)
        {
            return identityProvider == null ? null : IdentityProviderMapper.ToIdentityProviderDto(identityProvider);
        }

        public static IdentityProvidersDto ToModel(this PagedList<IdentityProvider> identityProvider)
        {
            if (identityProvider == null) return null;

            return new IdentityProvidersDto
            {
                TotalCount = identityProvider.TotalCount,
                PageSize = identityProvider.PageSize,
                IdentityProviders = identityProvider.Data.Select(IdentityProviderMapper.ToIdentityProviderDto).ToList()
            };
        }

        public static List<IdentityProviderDto> ToModel(this List<IdentityProvider> identityProvider)
        {
            return identityProvider?.Select(IdentityProviderMapper.ToIdentityProviderDto).ToList();
        }

        public static IdentityProvider ToEntity(this IdentityProviderDto identityProvider)
        {
            return identityProvider == null ? null : IdentityProviderMapper.ToIdentityProvider(identityProvider);
        }

        public static List<IdentityProvider> ToEntity(this List<IdentityProviderDto> identityProvider)
        {
            return identityProvider?.Select(IdentityProviderMapper.ToIdentityProvider).ToList();
        }
    }
}
