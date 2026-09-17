// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Mappers.Converters;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class ApiResourceMapper
    {
        public static partial ApiResourceDto ToApiResourceDto(ApiResource source);
        public static partial ApiResource ToApiResource(ApiResourceDto source);

        [MapProperty(nameof(ApiResourceSecret.Id), nameof(ApiSecretsDto.ApiSecretId))]
        public static partial ApiSecretsDto ToApiSecretsDto(ApiResourceSecret source);

        [MapProperty(nameof(ApiSecretsDto.ApiSecretId), nameof(ApiResourceSecret.Id))]
        public static partial ApiResourceSecret ToApiResourceSecret(ApiSecretsDto source);

        public static partial ApiSecretDto ToApiSecretDto(ApiResourceSecret source);

        [MapProperty(nameof(ApiResourceProperty.Id), nameof(ApiResourcePropertiesDto.ApiResourcePropertyId))]
        public static partial ApiResourcePropertiesDto ToApiResourcePropertiesDto(ApiResourceProperty source);

        [MapProperty(nameof(ApiResourcePropertiesDto.ApiResourcePropertyId), nameof(ApiResourceProperty.Id))]
        public static partial ApiResourceProperty ToApiResourceProperty(ApiResourcePropertiesDto source);

        public static partial ApiResourcePropertyDto ToApiResourcePropertyDto(ApiResourceProperty source);

        private static string MapApiResourceClaim(ApiResourceClaim source) => source.Type;
        private static ApiResourceClaim MapApiResourceClaim(string source) => new() { Type = source };

        private static string MapApiResourceScope(ApiResourceScope source) => source.Scope;
        private static ApiResourceScope MapApiResourceScope(string source) => new() { Scope = source };

        private static List<string> MapAllowedAccessTokenSigningAlgorithms(string source)
            => AllowedSigningAlgorithmsConverter.Converter.Convert(source) ?? [];

        private static string MapAllowedAccessTokenSigningAlgorithms(List<string> source)
            => AllowedSigningAlgorithmsConverter.Converter.Convert(source);
    }

    public static class ApiResourceMappers
    {
        public static ApiResourceDto ToModel(this ApiResource resource)
        {
            return resource == null ? null : ApiResourceMapper.ToApiResourceDto(resource);
        }

        public static ApiResourcesDto ToModel(this PagedList<ApiResource> resources)
        {
            if (resources == null) return null;

            return new ApiResourcesDto
            {
                TotalCount = resources.TotalCount,
                PageSize = resources.PageSize,
                ApiResources = resources.Data.Select(ApiResourceMapper.ToApiResourceDto).ToList()
            };
        }

        public static ApiResourcePropertiesDto ToModel(this PagedList<ApiResourceProperty> apiResourceProperties)
        {
            if (apiResourceProperties == null) return null;

            return new ApiResourcePropertiesDto
            {
                TotalCount = apiResourceProperties.TotalCount,
                PageSize = apiResourceProperties.PageSize,
                ApiResourceProperties = apiResourceProperties.Data.Select(ApiResourceMapper.ToApiResourcePropertyDto).ToList()
            };
        }

        public static ApiResourcePropertiesDto ToModel(this ApiResourceProperty apiResourceProperty)
        {
            if (apiResourceProperty == null) return null;

            var dto = ApiResourceMapper.ToApiResourcePropertiesDto(apiResourceProperty);
            dto.ApiResourceId = apiResourceProperty.ApiResource?.Id ?? default;
            return dto;
        }

        public static ApiSecretsDto ToModel(this PagedList<ApiResourceSecret> secrets)
        {
            if (secrets == null) return null;

            return new ApiSecretsDto
            {
                TotalCount = secrets.TotalCount,
                PageSize = secrets.PageSize,
                ApiSecrets = secrets.Data.Select(ApiResourceMapper.ToApiSecretDto).ToList()
            };
        }

        public static ApiSecretsDto ToModel(this ApiResourceSecret resource)
        {
            if (resource == null) return null;

            var dto = ApiResourceMapper.ToApiSecretsDto(resource);
            dto.ApiResourceId = resource.ApiResource?.Id ?? default;
            return dto;
        }

        public static ApiResource ToEntity(this ApiResourceDto resource)
        {
            return resource == null ? null : ApiResourceMapper.ToApiResource(resource);
        }

        public static ApiResourceSecret ToEntity(this ApiSecretsDto resource)
        {
            if (resource == null) return null;

            var entity = ApiResourceMapper.ToApiResourceSecret(resource);
            entity.ApiResource = new ApiResource { Id = resource.ApiResourceId };
            return entity;
        }

        public static ApiResourceProperty ToEntity(this ApiResourcePropertiesDto apiResourceProperties)
        {
            if (apiResourceProperties == null) return null;

            var entity = ApiResourceMapper.ToApiResourceProperty(apiResourceProperties);
            entity.ApiResource = new ApiResource { Id = apiResourceProperties.ApiResourceId };
            return entity;
        }
    }
}
