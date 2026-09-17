// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.IdentityResources;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class IdentityResourceApiMapper
    {
        public static partial IdentityResourcesApiDto ToIdentityResourcesApiDto(IdentityResourcesDto source);
        public static partial IdentityResourcesDto ToIdentityResourcesDto(IdentityResourcesApiDto source);

        public static partial IdentityResourceApiDto ToIdentityResourceApiDto(IdentityResourceDto source);
        public static partial IdentityResourceDto ToIdentityResourceDto(IdentityResourceApiDto source);

        [MapProperty(nameof(IdentityResourcePropertiesDto.IdentityResourcePropertyId), nameof(IdentityResourcePropertyApiDto.Id))]
        public static partial IdentityResourcePropertyApiDto ToIdentityResourcePropertyApiDto(IdentityResourcePropertiesDto source);

        [MapProperty(nameof(IdentityResourcePropertyApiDto.Id), nameof(IdentityResourcePropertiesDto.IdentityResourcePropertyId))]
        public static partial IdentityResourcePropertiesDto ToIdentityResourcePropertiesDto(IdentityResourcePropertyApiDto source);

        public static partial IdentityResourcePropertyApiDto ToIdentityResourcePropertyApiDto(IdentityResourcePropertyDto source);
        public static partial IdentityResourcePropertiesApiDto ToIdentityResourcePropertiesApiDto(IdentityResourcePropertiesDto source);
    }

    public static class IdentityResourceApiMappers
    {
        public static IdentityResourcesApiDto ToIdentityResourcesApiDto(this IdentityResourcesDto source) => IdentityResourceApiMapper.ToIdentityResourcesApiDto(source);
        public static IdentityResourcesDto ToIdentityResourcesDto(this IdentityResourcesApiDto source) => IdentityResourceApiMapper.ToIdentityResourcesDto(source);

        public static IdentityResourceApiDto ToIdentityResourceApiDto(this IdentityResourceDto source) => IdentityResourceApiMapper.ToIdentityResourceApiDto(source);
        public static IdentityResourceDto ToIdentityResourceDto(this IdentityResourceApiDto source) => IdentityResourceApiMapper.ToIdentityResourceDto(source);

        public static IdentityResourcePropertyApiDto ToIdentityResourcePropertyApiDto(this IdentityResourcePropertiesDto source) => IdentityResourceApiMapper.ToIdentityResourcePropertyApiDto(source);
        public static IdentityResourcePropertyApiDto ToIdentityResourcePropertyApiDto(this IdentityResourcePropertyDto source) => IdentityResourceApiMapper.ToIdentityResourcePropertyApiDto(source);
        public static IdentityResourcePropertiesDto ToIdentityResourcePropertiesDto(this IdentityResourcePropertyApiDto source) => IdentityResourceApiMapper.ToIdentityResourcePropertiesDto(source);
        public static IdentityResourcePropertiesApiDto ToIdentityResourcePropertiesApiDto(this IdentityResourcePropertiesDto source) => IdentityResourceApiMapper.ToIdentityResourcePropertiesApiDto(source);
    }
}
