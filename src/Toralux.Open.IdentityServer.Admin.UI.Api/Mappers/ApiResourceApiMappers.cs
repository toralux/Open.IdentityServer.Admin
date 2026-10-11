// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.ApiResources;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class ApiResourceApiMapper
    {
        public static partial ApiResourcesApiDto ToApiResourcesApiDto(ApiResourcesDto source);
        public static partial ApiResourcesDto ToApiResourcesDto(ApiResourcesApiDto source);

        public static partial ApiResourceApiDto ToApiResourceApiDto(ApiResourceDto source);
        public static partial ApiResourceDto ToApiResourceDto(ApiResourceApiDto source);

        [MapProperty(nameof(ApiSecretsDto.ApiSecretId), nameof(ApiSecretApiDto.Id))]
        public static partial ApiSecretApiDto ToApiSecretApiDto(ApiSecretsDto source);

        [MapProperty(nameof(ApiSecretApiDto.Id), nameof(ApiSecretsDto.ApiSecretId))]
        public static partial ApiSecretsDto ToApiSecretsDto(ApiSecretApiDto source);

        public static partial ApiSecretApiDto ToApiSecretApiDto(ApiSecretDto source);
        public static partial ApiSecretsApiDto ToApiSecretsApiDto(ApiSecretsDto source);

        [MapProperty(nameof(ApiResourcePropertiesDto.ApiResourcePropertyId), nameof(ApiResourcePropertyApiDto.Id))]
        public static partial ApiResourcePropertyApiDto ToApiResourcePropertyApiDto(ApiResourcePropertiesDto source);

        [MapProperty(nameof(ApiResourcePropertyApiDto.Id), nameof(ApiResourcePropertiesDto.ApiResourcePropertyId))]
        public static partial ApiResourcePropertiesDto ToApiResourcePropertiesDto(ApiResourcePropertyApiDto source);

        public static partial ApiResourcePropertyApiDto ToApiResourcePropertyApiDto(ApiResourcePropertyDto source);
        public static partial ApiResourcePropertiesApiDto ToApiResourcePropertiesApiDto(ApiResourcePropertiesDto source);
    }

    public static class ApiResourceApiMappers
    {
        public static ApiResourcesApiDto ToApiResourcesApiDto(this ApiResourcesDto source) => ApiResourceApiMapper.ToApiResourcesApiDto(source);
        public static ApiResourcesDto ToApiResourcesDto(this ApiResourcesApiDto source) => ApiResourceApiMapper.ToApiResourcesDto(source);

        public static ApiResourceApiDto ToApiResourceApiDto(this ApiResourceDto source) => ApiResourceApiMapper.ToApiResourceApiDto(source);
        public static ApiResourceDto ToApiResourceDto(this ApiResourceApiDto source) => ApiResourceApiMapper.ToApiResourceDto(source);

        public static ApiSecretApiDto ToApiSecretApiDto(this ApiSecretsDto source) => ApiResourceApiMapper.ToApiSecretApiDto(source);
        public static ApiSecretApiDto ToApiSecretApiDto(this ApiSecretDto source) => ApiResourceApiMapper.ToApiSecretApiDto(source);
        public static ApiSecretsDto ToApiSecretsDto(this ApiSecretApiDto source) => ApiResourceApiMapper.ToApiSecretsDto(source);
        public static ApiSecretsApiDto ToApiSecretsApiDto(this ApiSecretsDto source) => ApiResourceApiMapper.ToApiSecretsApiDto(source);

        public static ApiResourcePropertyApiDto ToApiResourcePropertyApiDto(this ApiResourcePropertiesDto source) => ApiResourceApiMapper.ToApiResourcePropertyApiDto(source);
        public static ApiResourcePropertyApiDto ToApiResourcePropertyApiDto(this ApiResourcePropertyDto source) => ApiResourceApiMapper.ToApiResourcePropertyApiDto(source);
        public static ApiResourcePropertiesDto ToApiResourcePropertiesDto(this ApiResourcePropertyApiDto source) => ApiResourceApiMapper.ToApiResourcePropertiesDto(source);
        public static ApiResourcePropertiesApiDto ToApiResourcePropertiesApiDto(this ApiResourcePropertiesDto source) => ApiResourceApiMapper.ToApiResourcePropertiesApiDto(source);
    }
}
