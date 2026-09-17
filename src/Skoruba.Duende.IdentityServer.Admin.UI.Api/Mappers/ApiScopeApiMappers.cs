// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Riok.Mapperly.Abstractions;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.Open.IdentityServer.Admin.UI.Api.Dtos.ApiScopes;

namespace Skoruba.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class ApiScopeApiMapper
    {
        public static partial ApiScopesApiDto ToApiScopesApiDto(ApiScopesDto source);
        public static partial ApiScopesDto ToApiScopesDto(ApiScopesApiDto source);

        public static partial ApiScopeApiDto ToApiScopeApiDto(ApiScopeDto source);
        public static partial ApiScopeDto ToApiScopeDto(ApiScopeApiDto source);

        public static partial ApiScopePropertiesApiDto ToApiScopePropertiesApiDto(ApiScopePropertiesDto source);
        public static partial ApiScopePropertiesDto ToApiScopePropertiesDto(ApiScopePropertiesApiDto source);

        public static partial ApiScopePropertyApiDto ToApiScopePropertyApiDto(ApiScopePropertyDto source);
        public static partial ApiScopePropertyDto ToApiScopePropertyDto(ApiScopePropertyApiDto source);

        [MapProperty(nameof(ApiScopePropertiesDto.ApiScopePropertyId), nameof(ApiScopePropertyApiDto.Id))]
        public static partial ApiScopePropertyApiDto ToApiScopePropertyApiDto(ApiScopePropertiesDto source);

        [MapProperty(nameof(ApiScopePropertyApiDto.Id), nameof(ApiScopePropertiesDto.ApiScopePropertyId))]
        public static partial ApiScopePropertiesDto ToApiScopePropertiesDto(ApiScopePropertyApiDto source);
    }

    public static class ApiScopeApiMappers
    {
        public static ApiScopesApiDto ToApiScopesApiDto(this ApiScopesDto source) => ApiScopeApiMapper.ToApiScopesApiDto(source);
        public static ApiScopesDto ToApiScopesDto(this ApiScopesApiDto source) => ApiScopeApiMapper.ToApiScopesDto(source);

        public static ApiScopeApiDto ToApiScopeApiDto(this ApiScopeDto source) => ApiScopeApiMapper.ToApiScopeApiDto(source);
        public static ApiScopeDto ToApiScopeDto(this ApiScopeApiDto source) => ApiScopeApiMapper.ToApiScopeDto(source);

        public static ApiScopePropertiesApiDto ToApiScopePropertiesApiDto(this ApiScopePropertiesDto source) => ApiScopeApiMapper.ToApiScopePropertiesApiDto(source);
        public static ApiScopePropertiesDto ToApiScopePropertiesDto(this ApiScopePropertiesApiDto source) => ApiScopeApiMapper.ToApiScopePropertiesDto(source);

        public static ApiScopePropertyApiDto ToApiScopePropertyApiDto(this ApiScopePropertyDto source) => ApiScopeApiMapper.ToApiScopePropertyApiDto(source);
        public static ApiScopePropertyApiDto ToApiScopePropertyApiDto(this ApiScopePropertiesDto source) => ApiScopeApiMapper.ToApiScopePropertyApiDto(source);

        public static ApiScopePropertyDto ToApiScopePropertyDto(this ApiScopePropertyApiDto source) => ApiScopeApiMapper.ToApiScopePropertyDto(source);
        public static ApiScopePropertiesDto ToApiScopePropertiesDto(this ApiScopePropertyApiDto source) => ApiScopeApiMapper.ToApiScopePropertiesDto(source);
    }
}
