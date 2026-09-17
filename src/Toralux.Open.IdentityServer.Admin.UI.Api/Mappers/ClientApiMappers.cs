// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Riok.Mapperly.Abstractions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Clients;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    internal static partial class ClientApiMapper
    {
        public static partial ClientApiDto ToClientApiDto(ClientDto source);
        public static partial ClientDto ToClientDto(ClientApiDto source);

        public static partial ClientsApiDto ToClientsApiDto(ClientsDto source);
        public static partial ClientsDto ToClientsDto(ClientsApiDto source);

        public static partial ClientCloneDto ToClientCloneDto(ClientCloneApiDto source);
        public static partial ClientCloneApiDto ToClientCloneApiDto(ClientCloneDto source);

        [MapProperty(nameof(ClientSecretsDto.ClientSecretId), nameof(ClientSecretApiDto.Id))]
        public static partial ClientSecretApiDto ToClientSecretApiDto(ClientSecretsDto source);

        [MapProperty(nameof(ClientSecretApiDto.Id), nameof(ClientSecretsDto.ClientSecretId))]
        public static partial ClientSecretsDto ToClientSecretsDto(ClientSecretApiDto source);

        public static partial ClientSecretApiDto ToClientSecretApiDto(ClientSecretDto source);
        public static partial ClientSecretsApiDto ToClientSecretsApiDto(ClientSecretsDto source);

        [MapProperty(nameof(ClientPropertiesDto.ClientPropertyId), nameof(ClientPropertyApiDto.Id))]
        public static partial ClientPropertyApiDto ToClientPropertyApiDto(ClientPropertiesDto source);

        [MapProperty(nameof(ClientPropertyApiDto.Id), nameof(ClientPropertiesDto.ClientPropertyId))]
        public static partial ClientPropertiesDto ToClientPropertiesDto(ClientPropertyApiDto source);

        public static partial ClientPropertyApiDto ToClientPropertyApiDto(ClientPropertyDto source);
        public static partial ClientPropertiesApiDto ToClientPropertiesApiDto(ClientPropertiesDto source);

        [MapProperty(nameof(ClientClaimsDto.ClientClaimId), nameof(ClientClaimApiDto.Id))]
        public static partial ClientClaimApiDto ToClientClaimApiDto(ClientClaimsDto source);

        [MapProperty(nameof(ClientClaimApiDto.Id), nameof(ClientClaimsDto.ClientClaimId))]
        public static partial ClientClaimsDto ToClientClaimsDto(ClientClaimApiDto source);

        public static partial ClientClaimApiDto ToClientClaimApiDto(ClientClaimDto source);
        public static partial ClientClaimsApiDto ToClientClaimsApiDto(ClientClaimsDto source);
    }

    public static class ClientApiMappers
    {
        public static ClientApiDto ToClientApiDto(this ClientDto source) => ClientApiMapper.ToClientApiDto(source);
        public static ClientDto ToClientDto(this ClientApiDto source) => ClientApiMapper.ToClientDto(source);

        public static ClientsApiDto ToClientsApiDto(this ClientsDto source) => ClientApiMapper.ToClientsApiDto(source);
        public static ClientsDto ToClientsDto(this ClientsApiDto source) => ClientApiMapper.ToClientsDto(source);

        public static ClientCloneDto ToClientCloneDto(this ClientCloneApiDto source) => ClientApiMapper.ToClientCloneDto(source);
        public static ClientCloneApiDto ToClientCloneApiDto(this ClientCloneDto source) => ClientApiMapper.ToClientCloneApiDto(source);

        public static ClientSecretApiDto ToClientSecretApiDto(this ClientSecretsDto source) => ClientApiMapper.ToClientSecretApiDto(source);
        public static ClientSecretApiDto ToClientSecretApiDto(this ClientSecretDto source) => ClientApiMapper.ToClientSecretApiDto(source);
        public static ClientSecretsDto ToClientSecretsDto(this ClientSecretApiDto source) => ClientApiMapper.ToClientSecretsDto(source);
        public static ClientSecretsApiDto ToClientSecretsApiDto(this ClientSecretsDto source) => ClientApiMapper.ToClientSecretsApiDto(source);

        public static ClientPropertyApiDto ToClientPropertyApiDto(this ClientPropertiesDto source) => ClientApiMapper.ToClientPropertyApiDto(source);
        public static ClientPropertyApiDto ToClientPropertyApiDto(this ClientPropertyDto source) => ClientApiMapper.ToClientPropertyApiDto(source);
        public static ClientPropertiesDto ToClientPropertiesDto(this ClientPropertyApiDto source) => ClientApiMapper.ToClientPropertiesDto(source);
        public static ClientPropertiesApiDto ToClientPropertiesApiDto(this ClientPropertiesDto source) => ClientApiMapper.ToClientPropertiesApiDto(source);

        public static ClientClaimApiDto ToClientClaimApiDto(this ClientClaimsDto source) => ClientApiMapper.ToClientClaimApiDto(source);
        public static ClientClaimApiDto ToClientClaimApiDto(this ClientClaimDto source) => ClientApiMapper.ToClientClaimApiDto(source);
        public static ClientClaimsDto ToClientClaimsDto(this ClientClaimApiDto source) => ClientApiMapper.ToClientClaimsDto(source);
        public static ClientClaimsApiDto ToClientClaimsApiDto(this ClientClaimsDto source) => ClientApiMapper.ToClientClaimsApiDto(source);
    }
}
