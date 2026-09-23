// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class IdentityResourceMapper
    {
        public static partial IdentityResourceDto ToIdentityResourceDto(IdentityResource source);
        public static partial IdentityResource ToIdentityResource(IdentityResourceDto source);

        [MapProperty(nameof(IdentityResourceProperty.Id), nameof(IdentityResourcePropertiesDto.IdentityResourcePropertyId))]
        public static partial IdentityResourcePropertiesDto ToIdentityResourcePropertiesDto(IdentityResourceProperty source);

        [MapProperty(nameof(IdentityResourcePropertiesDto.IdentityResourcePropertyId), nameof(IdentityResourceProperty.Id))]
        public static partial IdentityResourceProperty ToIdentityResourceProperty(IdentityResourcePropertiesDto source);

        public static partial IdentityResourcePropertyDto ToIdentityResourcePropertyDto(IdentityResourceProperty source);

        private static string MapIdentityResourceClaim(IdentityResourceClaim source) => source.Type;
        private static IdentityResourceClaim MapIdentityResourceClaim(string source) => new() { Type = source };
    }

    public static class IdentityResourceMappers
    {
        public static IdentityResourceDto ToModel(this IdentityResource resource)
        {
            return resource == null ? null : IdentityResourceMapper.ToIdentityResourceDto(resource);
        }

        public static IdentityResourcesDto ToModel(this PagedList<IdentityResource> resource)
        {
            if (resource == null) return null;

            return new IdentityResourcesDto
            {
                TotalCount = resource.TotalCount,
                PageSize = resource.PageSize,
                IdentityResources = resource.Data.Select(IdentityResourceMapper.ToIdentityResourceDto).ToList()
            };
        }

        public static List<IdentityResourceDto> ToModel(this List<IdentityResource> resource)
        {
            return resource?.Select(IdentityResourceMapper.ToIdentityResourceDto).ToList();
        }

        public static IdentityResource ToEntity(this IdentityResourceDto resource)
        {
            return resource == null ? null : IdentityResourceMapper.ToIdentityResource(resource);
        }

        public static IdentityResourcePropertiesDto ToModel(this PagedList<IdentityResourceProperty> identityResourceProperties)
        {
            if (identityResourceProperties == null) return null;

            return new IdentityResourcePropertiesDto
            {
                TotalCount = identityResourceProperties.TotalCount,
                PageSize = identityResourceProperties.PageSize,
                IdentityResourceProperties = identityResourceProperties.Data.Select(IdentityResourceMapper.ToIdentityResourcePropertyDto).ToList()
            };
        }

        public static IdentityResourcePropertiesDto ToModel(this IdentityResourceProperty identityResourceProperty)
        {
            if (identityResourceProperty == null) return null;

            var dto = IdentityResourceMapper.ToIdentityResourcePropertiesDto(identityResourceProperty);
            dto.IdentityResourceId = identityResourceProperty.IdentityResource?.Id ?? default;
            return dto;
        }

        public static List<IdentityResource> ToEntity(this List<IdentityResourceDto> resource)
        {
            return resource?.Select(IdentityResourceMapper.ToIdentityResource).ToList();
        }

        public static IdentityResourceProperty ToEntity(this IdentityResourcePropertiesDto identityResourceProperties)
        {
            if (identityResourceProperties == null) return null;

            var entity = IdentityResourceMapper.ToIdentityResourceProperty(identityResourceProperties);
            entity.IdentityResource = new IdentityResource { Id = identityResourceProperties.IdentityResourceId };
            return entity;
        }
    }
}
