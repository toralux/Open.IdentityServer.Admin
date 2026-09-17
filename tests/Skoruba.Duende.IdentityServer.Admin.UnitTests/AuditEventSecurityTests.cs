// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.ApiResource;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.ApiScope;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.Client;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityResource;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Events.Identity;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests
{
    public class AuditEventSecurityTests
    {
        [Fact]
        public void ClientEvents_RedactSecretsSaltAndPropertyValuesWithoutMutatingSource()
        {
            var source = CreateSensitiveClient();

            var auditClients = new[]
            {
                new ClientAddedEvent(source).Client,
                new ClientDeletedEvent(source).Client,
                new ClientRequestedEvent(source).ClientDto,
                new ClientUpdatedEvent(source, source).OriginalClient,
                new ClientUpdatedEvent(source, source).Client
            };

            foreach (var auditClient in auditClients)
            {
                AssertClientSanitized(auditClient);
                auditClient.Should().NotBeSameAs(source);
            }

            source.PairWiseSubjectSalt.Should().Be("pairwise-salt");
            source.ClientSecrets[0].Value.Should().Be("client-secret");
            source.Properties[0].Value.Should().Be("property-secret");
        }

        [Fact]
        public void ClientCloneAndListEvents_RedactSensitiveClientData()
        {
            var clone = new ClientCloneDto
            {
                PairWiseSubjectSalt = "clone-salt",
                ClientSecrets = new List<ClientSecretDto> { new() { Value = "clone-secret" } },
                Properties = new List<ClientPropertyDto> { new() { Value = "clone-property-secret" } }
            };
            var clients = new ClientsDto { Clients = new List<ClientDto> { CreateSensitiveClient() } };

            var clonedAuditClient = new ClientClonedEvent(clone).Client;
            var listedAuditClient = new ClientsRequestedEvent(clients).ClientsDto.Clients[0];

            AssertClientSanitized(clonedAuditClient);
            AssertClientSanitized(listedAuditClient);
            clone.PairWiseSubjectSalt.Should().Be("clone-salt");
            clients.Clients[0].PairWiseSubjectSalt.Should().Be("pairwise-salt");
        }

        [Fact]
        public void ClientPropertyEvents_RedactTopLevelAndCollectionValues()
        {
            var source = new ClientPropertiesDto
            {
                Value = "top-level-secret",
                ClientProperties = new List<ClientPropertyDto> { new() { Value = "collection-secret" } }
            };

            var payloads = new[]
            {
                new ClientPropertiesRequestedEvent(source).ClientProperties,
                new ClientPropertyAddedEvent(source).ClientProperties,
                new ClientPropertyDeletedEvent(source).ClientProperty,
                new ClientPropertyRequestedEvent(source).ClientProperties
            };

            foreach (var payload in payloads)
            {
                payload.Value.Should().BeNull();
                payload.ClientProperties[0].Value.Should().BeNull();
            }

            source.Value.Should().Be("top-level-secret");
            source.ClientProperties[0].Value.Should().Be("collection-secret");
        }

        [Fact]
        public void ApiResourcePropertyEvents_RedactTopLevelAndCollectionValues()
        {
            var source = new ApiResourcePropertiesDto
            {
                Value = "top-level-secret",
                ApiResourceProperties = new List<ApiResourcePropertyDto> { new() { Value = "collection-secret" } }
            };

            var payloads = new[]
            {
                new ApiResourcePropertiesRequestedEvent(1, source).ApiResourceProperties,
                new ApiResourcePropertyAddedEvent(source).ApiResourceProperty,
                new ApiResourcePropertyDeletedEvent(source).ApiResourceProperty,
                new ApiResourcePropertyRequestedEvent(1, source).ApiResourceProperties
            };

            foreach (var payload in payloads)
            {
                payload.Value.Should().BeNull();
                payload.ApiResourceProperties[0].Value.Should().BeNull();
            }

            source.Value.Should().Be("top-level-secret");
            source.ApiResourceProperties[0].Value.Should().Be("collection-secret");
        }

        [Fact]
        public void ApiScopePropertyEvents_RedactTopLevelAndCollectionValues()
        {
            var source = new ApiScopePropertiesDto
            {
                Value = "top-level-secret",
                ApiScopeProperties = new List<ApiScopePropertyDto> { new() { Value = "collection-secret" } }
            };

            var payloads = new[]
            {
                new ApiScopePropertiesRequestedEvent(1, source).ApiResourceProperties,
                new ApiScopePropertyAddedEvent(source).ApiScopeProperty,
                new ApiScopePropertyDeletedEvent(source).ApiScopeProperty,
                new ApiScopePropertyRequestedEvent(1, source).ApiScopeProperty
            };

            foreach (var payload in payloads)
            {
                payload.Value.Should().BeNull();
                payload.ApiScopeProperties[0].Value.Should().BeNull();
            }

            source.Value.Should().Be("top-level-secret");
            source.ApiScopeProperties[0].Value.Should().Be("collection-secret");
        }

        [Fact]
        public void ApiScopeEvents_RedactEmbeddedPropertyValuesWithoutMutatingSource()
        {
            var source = new ApiScopeDto
            {
                ApiScopeProperties = new List<ApiScopePropertyDto> { new() { Value = "scope-property-secret" } }
            };
            var scopes = new ApiScopesDto { Scopes = new List<ApiScopeDto> { source } };

            var payloads = new[]
            {
                new ApiScopeAddedEvent(source).ApiScope,
                new ApiScopeDeletedEvent(source).ApiScope,
                new ApiScopeRequestedEvent(source).ApiScopes,
                new ApiScopeUpdatedEvent(source, source).OriginalApiScope,
                new ApiScopeUpdatedEvent(source, source).ApiScope,
                new ApiScopesRequestedEvent(scopes).ApiScope.Scopes[0]
            };

            foreach (var payload in payloads)
            {
                payload.ApiScopeProperties[0].Value.Should().BeNull();
            }

            source.ApiScopeProperties[0].Value.Should().Be("scope-property-secret");
        }

        [Fact]
        public void IdentityResourcePropertyEvents_RedactTopLevelAndCollectionValues()
        {
            var source = new IdentityResourcePropertiesDto
            {
                Value = "top-level-secret",
                IdentityResourceProperties = new List<IdentityResourcePropertyDto> { new() { Value = "collection-secret" } }
            };

            var payloads = new[]
            {
                new IdentityResourcePropertiesRequestedEvent(source).IdentityResourceProperties,
                new IdentityResourcePropertyAddedEvent(source).IdentityResourceProperty,
                new IdentityResourcePropertyDeletedEvent(source).IdentityResourceProperty,
                new IdentityResourcePropertyRequestedEvent(source).IdentityResourceProperties
            };

            foreach (var payload in payloads)
            {
                payload.Value.Should().BeNull();
                payload.IdentityResourceProperties[0].Value.Should().BeNull();
            }

            source.Value.Should().Be("top-level-secret");
            source.IdentityResourceProperties[0].Value.Should().Be("collection-secret");
        }

        [Fact]
        public void IdentityUserListEvents_RedactKnownSensitiveFieldsWithoutMutatingSource()
        {
            var sourceUser = new SensitiveAuditUserDto
            {
                PasswordHash = "password-hash",
                SecurityStamp = "security-stamp",
                ConcurrencyStamp = "concurrency-stamp"
            };
            var source = new UsersDto<SensitiveAuditUserDto, string>
            {
                Users = new List<SensitiveAuditUserDto> { sourceUser }
            };

            var payloads = new[]
            {
                new UsersRequestedEvent<UsersDto<SensitiveAuditUserDto, string>>(source).Users,
                new RoleUsersRequestedEvent<UsersDto<SensitiveAuditUserDto, string>>(source).Users,
                new ClaimUsersRequestedEvent<UsersDto<SensitiveAuditUserDto, string>>(source).Users
            };

            foreach (var payload in payloads)
            {
                payload.Users[0].PasswordHash.Should().BeNull();
                payload.Users[0].SecurityStamp.Should().BeNull();
                payload.Users[0].ConcurrencyStamp.Should().BeNull();
            }

            sourceUser.PasswordHash.Should().Be("password-hash");
            sourceUser.SecurityStamp.Should().Be("security-stamp");
            sourceUser.ConcurrencyStamp.Should().Be("concurrency-stamp");
        }

        private static ClientDto CreateSensitiveClient()
        {
            return new ClientDto
            {
                PairWiseSubjectSalt = "pairwise-salt",
                ClientSecrets = new List<ClientSecretDto> { new() { Value = "client-secret" } },
                Properties = new List<ClientPropertyDto> { new() { Value = "property-secret" } }
            };
        }

        private static void AssertClientSanitized(ClientDto client)
        {
            client.PairWiseSubjectSalt.Should().BeNull();
            client.ClientSecrets[0].Value.Should().BeNull();
            client.Properties[0].Value.Should().BeNull();
        }

        private sealed class SensitiveAuditUserDto : UserDto<string>
        {
            public string PasswordHash { get; set; }
            public string SecurityStamp { get; set; }
            public string ConcurrencyStamp { get; set; }
        }
    }
}
