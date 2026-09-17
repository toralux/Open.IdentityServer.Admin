// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers.Converters;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared.Dtos.Common;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using DPoPTokenExpirationValidationMode = Toralux.Open.IdentityServer.Admin.EntityFramework.Constants.DPoPTokenExpirationValidationMode;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class ClientMapper
    {
        public static partial ClientDto ToClientDto(Client source);
        public static partial Client ToClient(ClientDto source);

        [MapProperty(nameof(ClientSecret.Id), nameof(ClientSecretsDto.ClientSecretId))]
        public static partial ClientSecretsDto ToClientSecretsDto(ClientSecret source);

        [MapProperty(nameof(ClientSecretsDto.ClientSecretId), nameof(ClientSecret.Id))]
        public static partial ClientSecret ToClientSecret(ClientSecretsDto source);

        public static partial ClientSecretDto ToClientSecretDto(ClientSecret source);
        public static partial ClientSecret ToClientSecret(ClientSecretDto source);

        [MapProperty(nameof(ClientClaim.Id), nameof(ClientClaimsDto.ClientClaimId))]
        public static partial ClientClaimsDto ToClientClaimsDto(ClientClaim source);

        /// <summary>
        /// Maps a single claim row DTO used by claim CRUD endpoints and preserves the original claim identifier.
        /// </summary>
        [MapProperty(nameof(ClientClaimsDto.ClientClaimId), nameof(ClientClaim.Id))]
        public static partial ClientClaim ToClientClaim(ClientClaimsDto source);

        public static partial ClientClaimDto ToClientClaimDto(ClientClaim source);

        /// <summary>
        /// Maps claims from the full client graph update payload and intentionally ignores identifier to avoid EF tracking conflicts.
        /// </summary>
        [MapperIgnoreTarget(nameof(ClientClaim.Id))]
        public static partial ClientClaim ToClientClaim(ClientClaimDto source);

        [MapProperty(nameof(ClientProperty.Id), nameof(ClientPropertiesDto.ClientPropertyId))]
        public static partial ClientPropertiesDto ToClientPropertiesDto(ClientProperty source);

        [MapProperty(nameof(ClientPropertiesDto.ClientPropertyId), nameof(ClientProperty.Id))]
        public static partial ClientProperty ToClientProperty(ClientPropertiesDto source);

        public static partial ClientPropertyDto ToClientPropertyDto(ClientProperty source);
        public static partial ClientProperty ToClientProperty(ClientPropertyDto source);

        private static string MapClientGrantType(ClientGrantType source) => source.GrantType;
        private static ClientGrantType MapClientGrantType(string source) => new() { GrantType = source };

        private static string MapClientRedirectUri(ClientRedirectUri source) => source.RedirectUri;
        private static ClientRedirectUri MapClientRedirectUri(string source) => new() { RedirectUri = source };

        private static string MapClientPostLogoutRedirectUri(ClientPostLogoutRedirectUri source) => source.PostLogoutRedirectUri;
        private static ClientPostLogoutRedirectUri MapClientPostLogoutRedirectUri(string source) => new() { PostLogoutRedirectUri = source };

        private static string MapClientScope(ClientScope source) => source.Scope;
        private static ClientScope MapClientScope(string source) => new() { Scope = source };

        private static string MapClientIdPRestriction(ClientIdPRestriction source) => source.Provider;
        private static ClientIdPRestriction MapClientIdPRestriction(string source) => new() { Provider = source };

        private static string MapClientCorsOrigin(ClientCorsOrigin source) => source.Origin;
        private static ClientCorsOrigin MapClientCorsOrigin(string source) => new() { Origin = source };

        private static List<string> MapAllowedIdentityTokenSigningAlgorithms(string source)
            => AllowedSigningAlgorithmsConverter.Converter.Convert(source) ?? [];

        private static string MapAllowedIdentityTokenSigningAlgorithms(List<string> source)
            => AllowedSigningAlgorithmsConverter.Converter.Convert(source);

        private static int MapDPoPValidationMode(DPoPTokenExpirationValidationMode source) => (int)source;
        private static DPoPTokenExpirationValidationMode MapDPoPValidationMode(int source) => (DPoPTokenExpirationValidationMode)source;
    }

    public static class ClientMappers
    {
        public static ClientDto ToModel(this Client client)
        {
            if (client == null) return null;

            var dto = ClientMapper.ToClientDto(client);
            dto.CoordinateLifetimeWithUserSession = client.CoordinateLifetimeWithUserSession ?? false;
            return dto;
        }

        public static ClientSecretsDto ToModel(this PagedList<ClientSecret> clientSecret)
        {
            if (clientSecret == null) return null;

            return new ClientSecretsDto
            {
                TotalCount = clientSecret.TotalCount,
                PageSize = clientSecret.PageSize,
                ClientSecrets = clientSecret.Data.Select(ClientMapper.ToClientSecretDto).ToList()
            };
        }

        public static ClientClaimsDto ToModel(this PagedList<ClientClaim> clientClaims)
        {
            if (clientClaims == null) return null;

            return new ClientClaimsDto
            {
                TotalCount = clientClaims.TotalCount,
                PageSize = clientClaims.PageSize,
                ClientClaims = clientClaims.Data.Select(ClientMapper.ToClientClaimDto).ToList()
            };
        }

        public static ClientsDto ToModel(this PagedList<Client> clients)
        {
            if (clients == null) return null;

            return new ClientsDto
            {
                TotalCount = clients.TotalCount,
                PageSize = clients.PageSize,
                Clients = clients.Data.Select(ToModel).ToList()
            };
        }

        public static ClientPropertiesDto ToModel(this PagedList<ClientProperty> clientProperties)
        {
            if (clientProperties == null) return null;

            return new ClientPropertiesDto
            {
                TotalCount = clientProperties.TotalCount,
                PageSize = clientProperties.PageSize,
                ClientProperties = clientProperties.Data.Select(ClientMapper.ToClientPropertyDto).ToList()
            };
        }

        public static Client ToEntity(this ClientDto client)
        {
            if (client == null) return null;

            var entity = ClientMapper.ToClient(client);
            entity.CoordinateLifetimeWithUserSession = client.CoordinateLifetimeWithUserSession;
            return entity;
        }

        public static ClientSecretsDto ToModel(this ClientSecret clientSecret)
        {
            if (clientSecret == null) return null;

            var dto = ClientMapper.ToClientSecretsDto(clientSecret);
            dto.ClientId = clientSecret.Client?.Id ?? default;
            return dto;
        }

        public static ClientSecret ToEntity(this ClientSecretsDto clientSecret)
        {
            if (clientSecret == null) return null;

            var entity = ClientMapper.ToClientSecret(clientSecret);
            entity.Client = new Client { Id = clientSecret.ClientId };
            return entity;
        }

        public static ClientClaimsDto ToModel(this ClientClaim clientClaim)
        {
            if (clientClaim == null) return null;

            var dto = ClientMapper.ToClientClaimsDto(clientClaim);
            dto.ClientId = clientClaim.Client?.Id ?? default;
            return dto;
        }

        public static ClientPropertiesDto ToModel(this ClientProperty clientProperty)
        {
            if (clientProperty == null) return null;

            var dto = ClientMapper.ToClientPropertiesDto(clientProperty);
            dto.ClientId = clientProperty.Client?.Id ?? default;
            return dto;
        }

        public static ClientClaim ToEntity(this ClientClaimsDto clientClaim)
        {
            if (clientClaim == null) return null;

            var entity = ClientMapper.ToClientClaim(clientClaim);
            entity.Client = new Client { Id = clientClaim.ClientId };
            return entity;
        }

        public static ClientProperty ToEntity(this ClientPropertiesDto clientProperties)
        {
            if (clientProperties == null) return null;

            var entity = ClientMapper.ToClientProperty(clientProperties);
            entity.Client = new Client { Id = clientProperties.ClientId };
            return entity;
        }

        public static SelectItemDto ToModel(this SelectItem selectItem)
        {
            return selectItem == null ? null : new SelectItemDto(selectItem.Id, selectItem.Text);
        }

        public static List<SelectItemDto> ToModel(this List<SelectItem> selectItem)
        {
            return selectItem?.Select(ToModel).ToList() ?? [];
        }
    }
}
