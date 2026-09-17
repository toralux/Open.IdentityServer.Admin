// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class ApiScopeMapper
    {
        public static partial ApiScopeDto ToApiScopeDto(ApiScope source);
        public static partial ApiScope ToApiScope(ApiScopeDto source);

        [MapProperty(nameof(ApiScopeProperty.Id), nameof(ApiScopePropertiesDto.ApiScopePropertyId))]
        [MapProperty(nameof(ApiScopeProperty.ScopeId), nameof(ApiScopePropertiesDto.ApiScopeId))]
        public static partial ApiScopePropertiesDto ToApiScopePropertiesDto(ApiScopeProperty source);

        [MapProperty(nameof(ApiScopePropertiesDto.ApiScopePropertyId), nameof(ApiScopeProperty.Id))]
        [MapProperty(nameof(ApiScopePropertiesDto.ApiScopeId), nameof(ApiScopeProperty.ScopeId))]
        public static partial ApiScopeProperty ToApiScopeProperty(ApiScopePropertiesDto source);

        public static partial ApiScopePropertyDto ToApiScopePropertyDto(ApiScopeProperty source);

        private static string MapApiScopeClaim(ApiScopeClaim source) => source.Type;
        private static ApiScopeClaim MapApiScopeClaim(string source) => new() { Type = source };
    }

    public static class ApiScopeMappers
    {
        public static ApiScopesDto ToModel(this PagedList<ApiScope> scopes)
        {
            if (scopes == null) return null;

            return new ApiScopesDto
            {
                TotalCount = scopes.TotalCount,
                PageSize = scopes.PageSize,
                Scopes = scopes.Data.Select(ApiScopeMapper.ToApiScopeDto).ToList()
            };
        }

        public static ApiScopeDto ToModel(this ApiScope resource)
        {
            return resource == null ? null : ApiScopeMapper.ToApiScopeDto(resource);
        }

        public static ApiScope ToEntity(this ApiScopeDto resource)
        {
            return resource == null ? null : ApiScopeMapper.ToApiScope(resource);
        }

        public static ApiScopeProperty ToEntity(this ApiScopePropertiesDto resource)
        {
            if (resource == null) return null;

            var entity = ApiScopeMapper.ToApiScopeProperty(resource);
            entity.Scope = new ApiScope { Id = resource.ApiScopeId };
            return entity;
        }

        public static ApiScopePropertiesDto ToModel(this PagedList<ApiScopeProperty> scope)
        {
            if (scope == null) return null;

            return new ApiScopePropertiesDto
            {
                TotalCount = scope.TotalCount,
                PageSize = scope.PageSize,
                ApiScopeProperties = scope.Data.Select(ApiScopeMapper.ToApiScopePropertyDto).ToList()
            };
        }

        public static ApiScopePropertiesDto ToModel(this ApiScopeProperty scope)
        {
            if (scope == null) return null;

            var dto = ApiScopeMapper.ToApiScopePropertiesDto(scope);
            dto.ApiScopeId = scope.Scope?.Id ?? scope.ScopeId;
            return dto;
        }
    }
}
