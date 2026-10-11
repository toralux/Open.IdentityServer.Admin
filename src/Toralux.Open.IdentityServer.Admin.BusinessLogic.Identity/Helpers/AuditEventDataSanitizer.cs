// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Grant;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Helpers
{
    public static class AuditEventDataSanitizer
    {
        private static readonly HashSet<string> SensitiveUserPropertyNames = new(StringComparer.Ordinal)
        {
            "PasswordHash",
            "SecurityStamp",
            "ConcurrencyStamp"
        };

        private static readonly JsonSerializerOptions CloneOptions = new()
        {
            PropertyNameCaseInsensitive = false
        };

        public static TUserDto SanitizeUser<TUserDto>(TUserDto user)
        {
            var sanitizedUser = Clone(user);
            if (sanitizedUser == null)
            {
                return default;
            }

            SanitizeSensitiveUserProperties(sanitizedUser);
            return sanitizedUser;
        }

        public static TUsersDto SanitizeUsers<TUsersDto, TUserDto, TKey>(TUsersDto users)
            where TUsersDto : UsersDto<TUserDto, TKey>
            where TUserDto : UserDto<TKey>
            where TKey : IEquatable<TKey>
        {
            return SanitizeUsers<TUsersDto>(users);
        }

        public static TUsersDto SanitizeUsers<TUsersDto>(TUsersDto users)
        {
            var sanitizedUsers = Clone(users);
            if (sanitizedUsers == null)
            {
                return sanitizedUsers;
            }

            var usersProperty = sanitizedUsers.GetType().GetProperty("Users", BindingFlags.Instance | BindingFlags.Public);
            if (usersProperty?.GetValue(sanitizedUsers) is not IEnumerable userItems)
            {
                return sanitizedUsers;
            }

            foreach (var user in userItems)
            {
                if (user != null)
                {
                    SanitizeSensitiveUserProperties(user);
                }
            }

            return sanitizedUsers;
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

        private static void SanitizeSensitiveUserProperties<TUserDto>(TUserDto user)
        {
            foreach (var property in user.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanRead || !property.CanWrite || !SensitiveUserPropertyNames.Contains(property.Name))
                {
                    continue;
                }

                var propertyType = property.PropertyType;
                if (propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) == null)
                {
                    continue;
                }

                property.SetValue(user, null);
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
