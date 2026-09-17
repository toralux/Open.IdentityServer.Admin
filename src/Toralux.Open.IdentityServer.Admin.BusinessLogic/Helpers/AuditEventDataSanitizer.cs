// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Helpers
{
    public static class AuditEventDataSanitizer
    {
        private static readonly JsonSerializerOptions CloneOptions = new()
        {
            PropertyNameCaseInsensitive = false
        };

        public static ClientDto Sanitize(ClientDto client)
        {
            var sanitizedClient = Clone(client);
            SanitizeClient(sanitizedClient);
            return sanitizedClient;
        }

        public static ClientCloneDto Sanitize(ClientCloneDto client)
        {
            var sanitizedClient = Clone(client);
            SanitizeClient(sanitizedClient);
            return sanitizedClient;
        }

        public static ClientsDto Sanitize(ClientsDto clients)
        {
            var sanitizedClients = Clone(clients);
            if (sanitizedClients?.Clients == null)
            {
                return sanitizedClients;
            }

            foreach (var client in sanitizedClients.Clients)
            {
                SanitizeClient(client);
            }

            return sanitizedClients;
        }

        public static ClientSecretsDto Sanitize(ClientSecretsDto clientSecrets)
        {
            var sanitizedClientSecrets = Clone(clientSecrets);
            if (sanitizedClientSecrets == null)
            {
                return null;
            }

            sanitizedClientSecrets.Value = null;

            if (sanitizedClientSecrets.ClientSecrets == null)
            {
                return sanitizedClientSecrets;
            }

            foreach (var secret in sanitizedClientSecrets.ClientSecrets)
            {
                if (secret != null)
                {
                    secret.Value = null;
                }
            }

            return sanitizedClientSecrets;
        }

        public static PersistedGrantDto Sanitize(PersistedGrantDto persistedGrant)
        {
            var sanitizedGrant = Clone(persistedGrant);
            if (sanitizedGrant == null)
            {
                return null;
            }

            sanitizedGrant.Data = null;
            sanitizedGrant.SessionId = null;
            return sanitizedGrant;
        }

        public static PersistedGrantsDto Sanitize(PersistedGrantsDto persistedGrants)
        {
            var sanitizedGrants = Clone(persistedGrants);
            if (sanitizedGrants?.PersistedGrants == null)
            {
                return sanitizedGrants;
            }

            foreach (var persistedGrant in sanitizedGrants.PersistedGrants)
            {
                if (persistedGrant != null)
                {
                    persistedGrant.Data = null;
                    persistedGrant.SessionId = null;
                }
            }

            return sanitizedGrants;
        }

        public static ApiSecretsDto Sanitize(ApiSecretsDto apiSecrets)
        {
            var sanitizedApiSecrets = Clone(apiSecrets);
            if (sanitizedApiSecrets == null)
            {
                return null;
            }

            sanitizedApiSecrets.Value = null;

            if (sanitizedApiSecrets.ApiSecrets == null)
            {
                return sanitizedApiSecrets;
            }

            foreach (var secret in sanitizedApiSecrets.ApiSecrets)
            {
                if (secret != null)
                {
                    secret.Value = null;
                }
            }

            return sanitizedApiSecrets;
        }

        public static ApiScopeDto Sanitize(ApiScopeDto apiScope)
        {
            var sanitizedApiScope = Clone(apiScope);
            SanitizeApiScope(sanitizedApiScope);
            return sanitizedApiScope;
        }

        public static ApiScopesDto Sanitize(ApiScopesDto apiScopes)
        {
            var sanitizedApiScopes = Clone(apiScopes);
            if (sanitizedApiScopes?.Scopes == null)
            {
                return sanitizedApiScopes;
            }

            foreach (var apiScope in sanitizedApiScopes.Scopes)
            {
                SanitizeApiScope(apiScope);
            }

            return sanitizedApiScopes;
        }

        public static ClientPropertiesDto Sanitize(ClientPropertiesDto properties)
        {
            var sanitizedProperties = Clone(properties);
            if (sanitizedProperties == null)
            {
                return null;
            }

            sanitizedProperties.Value = null;
            if (sanitizedProperties.ClientProperties != null)
            {
                foreach (var property in sanitizedProperties.ClientProperties)
                {
                    if (property != null)
                    {
                        property.Value = null;
                    }
                }
            }

            return sanitizedProperties;
        }

        public static ApiResourcePropertiesDto Sanitize(ApiResourcePropertiesDto properties)
        {
            var sanitizedProperties = Clone(properties);
            if (sanitizedProperties == null)
            {
                return null;
            }

            sanitizedProperties.Value = null;
            if (sanitizedProperties.ApiResourceProperties != null)
            {
                foreach (var property in sanitizedProperties.ApiResourceProperties)
                {
                    if (property != null)
                    {
                        property.Value = null;
                    }
                }
            }

            return sanitizedProperties;
        }

        public static ApiScopePropertiesDto Sanitize(ApiScopePropertiesDto properties)
        {
            var sanitizedProperties = Clone(properties);
            if (sanitizedProperties == null)
            {
                return null;
            }

            sanitizedProperties.Value = null;
            if (sanitizedProperties.ApiScopeProperties != null)
            {
                foreach (var property in sanitizedProperties.ApiScopeProperties)
                {
                    if (property != null)
                    {
                        property.Value = null;
                    }
                }
            }

            return sanitizedProperties;
        }

        public static IdentityResourcePropertiesDto Sanitize(IdentityResourcePropertiesDto properties)
        {
            var sanitizedProperties = Clone(properties);
            if (sanitizedProperties == null)
            {
                return null;
            }

            sanitizedProperties.Value = null;
            if (sanitizedProperties.IdentityResourceProperties != null)
            {
                foreach (var property in sanitizedProperties.IdentityResourceProperties)
                {
                    if (property != null)
                    {
                        property.Value = null;
                    }
                }
            }

            return sanitizedProperties;
        }

        private static void SanitizeClient(ClientDto client)
        {
            if (client == null)
            {
                return;
            }

            client.PairWiseSubjectSalt = null;

            if (client.ClientSecrets != null)
            {
                foreach (var secret in client.ClientSecrets)
                {
                    if (secret != null)
                    {
                        secret.Value = null;
                    }
                }
            }

            if (client.Properties != null)
            {
                foreach (var property in client.Properties)
                {
                    if (property != null)
                    {
                        property.Value = null;
                    }
                }
            }
        }

        private static void SanitizeApiScope(ApiScopeDto apiScope)
        {
            if (apiScope?.ApiScopeProperties == null)
            {
                return;
            }

            foreach (var property in apiScope.ApiScopeProperties)
            {
                if (property != null)
                {
                    property.Value = null;
                }
            }
        }

        private static T Clone<T>(T source)
        {
            if (source == null)
            {
                return default;
            }

            var serialized = JsonSerializer.Serialize(source, CloneOptions);
            return JsonSerializer.Deserialize<T>(serialized, CloneOptions);
        }
    }
}
