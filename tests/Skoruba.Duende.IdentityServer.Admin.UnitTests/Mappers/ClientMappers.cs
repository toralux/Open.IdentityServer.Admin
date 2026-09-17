// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers.Converters;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Mappers
{
    public class ClientMappers
    {
        [Fact]
        public void CanMapClientToModel()
        {
            //Generate entity
            var client = ClientMock.GenerateRandomClient(0);

            //Try map to DTO
            var clientDto = client.ToModel();

            //Asert
            clientDto.Should().NotBeNull();

            clientDto.Should().BeEquivalentTo(client, options =>
                options.Excluding(o => o.AllowedCorsOrigins)
                       .Excluding(o => o.RedirectUris)
                       .Excluding(o => o.PostLogoutRedirectUris)
                       .Excluding(o => o.AllowedGrantTypes)
                       .Excluding(o => o.AllowedScopes)
					   .Excluding(o => o.Created)
                       .Excluding(o => o.DPoPValidationMode)
                       .Excluding(o => o.AllowedIdentityTokenSigningAlgorithms)
                       .Excluding(o => o.IdentityProviderRestrictions));

            clientDto.DPoPValidationMode.Should().Be((int)client.DPoPValidationMode);
            
            //Assert collection
            clientDto.AllowedCorsOrigins.Should().BeEquivalentTo(client.AllowedCorsOrigins.Select(x => x.Origin));
            clientDto.RedirectUris.Should().BeEquivalentTo(client.RedirectUris.Select(x => x.RedirectUri));
            clientDto.PostLogoutRedirectUris.Should().BeEquivalentTo(client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri));
            clientDto.AllowedGrantTypes.Should().BeEquivalentTo(client.AllowedGrantTypes.Select(x => x.GrantType));
            clientDto.AllowedScopes.Should().BeEquivalentTo(client.AllowedScopes.Select(x => x.Scope));
            clientDto.IdentityProviderRestrictions.Should().BeEquivalentTo(client.IdentityProviderRestrictions.Select(x => x.Provider));
            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(client.AllowedIdentityTokenSigningAlgorithms, null);
            clientDto.AllowedIdentityTokenSigningAlgorithms.Should().BeEquivalentTo(allowedAlgList);
        }

        [Fact]
        public void CanMapClientDtoToEntity()
        {
            //Generate DTO
            var clientDto = ClientDtoMock.GenerateRandomClient(0);

            //Try map to entity
            var client = clientDto.ToEntity();

            client.Should().NotBeNull();

            clientDto.Should().BeEquivalentTo(client, options =>
                options.Excluding(o => o.AllowedCorsOrigins)
                    .Excluding(o => o.RedirectUris)
                    .Excluding(o => o.PostLogoutRedirectUris)
                    .Excluding(o => o.AllowedGrantTypes)
                    .Excluding(o => o.AllowedScopes)
                    .Excluding(o => o.AllowedIdentityTokenSigningAlgorithms)
                    .Excluding(o => o.Created)
                    .Excluding(o => o.DPoPValidationMode)
					.Excluding(o => o.IdentityProviderRestrictions));

            clientDto.DPoPValidationMode.Should().Be((int)client.DPoPValidationMode);

            //Assert collection
            clientDto.AllowedCorsOrigins.Should().BeEquivalentTo(client.AllowedCorsOrigins.Select(x => x.Origin));
            clientDto.RedirectUris.Should().BeEquivalentTo(client.RedirectUris.Select(x => x.RedirectUri));
            clientDto.PostLogoutRedirectUris.Should().BeEquivalentTo(client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri));
            clientDto.AllowedGrantTypes.Should().BeEquivalentTo(client.AllowedGrantTypes.Select(x => x.GrantType));
            clientDto.AllowedScopes.Should().BeEquivalentTo(client.AllowedScopes.Select(x => x.Scope));
            clientDto.IdentityProviderRestrictions.Should().BeEquivalentTo(client.IdentityProviderRestrictions.Select(x => x.Provider));
            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(client.AllowedIdentityTokenSigningAlgorithms, null);
            clientDto.AllowedIdentityTokenSigningAlgorithms.Should().BeEquivalentTo(allowedAlgList);
        }

        [Fact]
        public void CanMapClientClaimToModel()
        {
            var clientClaim = ClientMock.GenerateRandomClientClaim(0);

            var clientClaimsDto = clientClaim.ToModel();

            //Assert
            clientClaimsDto.Should().NotBeNull();

            clientClaimsDto.Should().BeEquivalentTo(clientClaim, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void MapClientClaimToModel_MapsId()
        {
            var client = ClientMock.GenerateRandomClient(1);
            client.Claims.Add(ClientMock.GenerateRandomClientClaim(42));

            var clientDto = client.ToModel();

            clientDto.Claims.Should().HaveCount(1);
            clientDto.Claims[0].Id.Should().Be(42);
            clientDto.Claims[0].Type.Should().Be(client.Claims[0].Type);
            clientDto.Claims[0].Value.Should().Be(client.Claims[0].Value);
        }

        [Fact]
        public void CanMapClientClaimToEntity()
        {
            var clientClaimDto = ClientDtoMock.GenerateRandomClientClaim(0, 0);

            var clientClaim = clientClaimDto.ToEntity();

            //Assert
            clientClaim.Should().NotBeNull();

            clientClaimDto.Should().BeEquivalentTo(clientClaim, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void MapClientClaimToEntity_IgnoresId()
        {
            var clientDto = ClientDtoMock.GenerateRandomClient(1);
            var claimDto = new Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration.ClientClaimDto
            {
                Id = 42,
                Type = "sub",
                Value = "value"
            };
            clientDto.Claims.Add(claimDto);

            var client = clientDto.ToEntity();

            client.Claims.Should().HaveCount(1);
            client.Claims[0].Id.Should().Be(0);
            client.Claims[0].Type.Should().Be(claimDto.Type);
            client.Claims[0].Value.Should().Be(claimDto.Value);
        }

        [Fact]
        public void CanMapClientSecretToModel()
        {
            var clientSecret = ClientMock.GenerateRandomClientSecret(0);

            var clientSecretsDto = clientSecret.ToModel();

            //Assert
            clientSecretsDto.Should().NotBeNull();

            clientSecretsDto.Should().BeEquivalentTo(clientSecret, options =>
                options.Excluding(o => o.Id)
	                .Excluding(o => o.Created)
					.Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientSecretToEntity()
        {
            var clientSecretsDto = ClientDtoMock.GenerateRandomClientSecret(0, 0);

            var clientSecret = clientSecretsDto.ToEntity();

            //Assert
            clientSecret.Should().NotBeNull();

            clientSecretsDto.Should().BeEquivalentTo(clientSecret, options =>
                options.Excluding(o => o.Id)
	                .Excluding(o => o.Created)
					.Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientPropertyToModel()
        {
            var clientProperty = ClientMock.GenerateRandomClientProperty(0);

            var clientPropertiesDto = clientProperty.ToModel();

            //Assert
            clientPropertiesDto.Should().NotBeNull();

            clientPropertiesDto.Should().BeEquivalentTo(clientProperty, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapClientPropertyToEntity()
        {
            var clientPropertiesDto = ClientDtoMock.GenerateRandomClientProperty(0, 0);

            var clientProperty = clientPropertiesDto.ToEntity();

            //Assert
            clientProperty.Should().NotBeNull();

            clientPropertiesDto.Should().BeEquivalentTo(clientProperty, options =>
                options.Excluding(o => o.Id)
                    .Excluding(o => o.Client));
        }

        [Fact]
        public void CanMapPagedClientsToModel()
        {
            var clients = new PagedList<Client>
            {
                TotalCount = 10,
                PageSize = 5
            };
            clients.Data.Add(ClientMock.GenerateRandomClient(1));
            clients.Data.Add(ClientMock.GenerateRandomClient(2));

            var clientsDto = clients.ToModel();

            clientsDto.Should().NotBeNull();
            clientsDto.TotalCount.Should().Be(clients.TotalCount);
            clientsDto.PageSize.Should().Be(clients.PageSize);
            clientsDto.Clients.Should().HaveCount(clients.Data.Count);
            clientsDto.Clients.Select(x => x.ClientId).Should().BeEquivalentTo(clients.Data.Select(x => x.ClientId));
        }

        [Fact]
        public void CanMapPagedClientSecretsToModel()
        {
            var secrets = new PagedList<ClientSecret>
            {
                TotalCount = 7,
                PageSize = 3
            };
            secrets.Data.Add(ClientMock.GenerateRandomClientSecret(1));
            secrets.Data.Add(ClientMock.GenerateRandomClientSecret(2));

            var secretsDto = secrets.ToModel();

            secretsDto.Should().NotBeNull();
            secretsDto.TotalCount.Should().Be(secrets.TotalCount);
            secretsDto.PageSize.Should().Be(secrets.PageSize);
            secretsDto.ClientSecrets.Should().HaveCount(secrets.Data.Count);
        }

        [Fact]
        public void CanMapPagedClientClaimsToModel()
        {
            var claims = new PagedList<ClientClaim>
            {
                TotalCount = 6,
                PageSize = 2
            };
            claims.Data.Add(ClientMock.GenerateRandomClientClaim(1));
            claims.Data.Add(ClientMock.GenerateRandomClientClaim(2));

            var claimsDto = claims.ToModel();

            claimsDto.Should().NotBeNull();
            claimsDto.TotalCount.Should().Be(claims.TotalCount);
            claimsDto.PageSize.Should().Be(claims.PageSize);
            claimsDto.ClientClaims.Should().HaveCount(claims.Data.Count);
        }

        [Fact]
        public void CanMapPagedClientPropertiesToModel()
        {
            var properties = new PagedList<ClientProperty>
            {
                TotalCount = 4,
                PageSize = 2
            };
            properties.Data.Add(ClientMock.GenerateRandomClientProperty(1));
            properties.Data.Add(ClientMock.GenerateRandomClientProperty(2));

            var propertiesDto = properties.ToModel();

            propertiesDto.Should().NotBeNull();
            propertiesDto.TotalCount.Should().Be(properties.TotalCount);
            propertiesDto.PageSize.Should().Be(properties.PageSize);
            propertiesDto.ClientProperties.Should().HaveCount(properties.Data.Count);
        }
    }
}
