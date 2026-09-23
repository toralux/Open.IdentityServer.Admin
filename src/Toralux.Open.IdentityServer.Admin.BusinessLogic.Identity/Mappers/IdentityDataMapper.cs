// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Helpers;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers
{
    public class IdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
        TUserClaimDto, TRoleClaimDto>
        : IIdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto>,
          IIdentityAuditDataMapper<TUserDto, TUsersDto>
        where TUserDto : UserDto<TKey>
        where TRoleDto : RoleDto<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
    {
        private static readonly HashSet<string> UserCustomCopyExcludedPropertyNames = new(StringComparer.Ordinal)
        {
            nameof(IdentityUser<TKey>.Id),
            nameof(IdentityUser<TKey>.UserName),
            nameof(IdentityUser<TKey>.Email),
            nameof(IdentityUser<TKey>.EmailConfirmed),
            nameof(IdentityUser<TKey>.PhoneNumber),
            nameof(IdentityUser<TKey>.PhoneNumberConfirmed),
            nameof(IdentityUser<TKey>.LockoutEnabled),
            nameof(IdentityUser<TKey>.LockoutEnd),
            nameof(IdentityUser<TKey>.TwoFactorEnabled),
            nameof(IdentityUser<TKey>.AccessFailedCount),
            nameof(IdentityUser<TKey>.NormalizedUserName),
            nameof(IdentityUser<TKey>.NormalizedEmail),
            nameof(IdentityUser<TKey>.PasswordHash),
            nameof(IdentityUser<TKey>.SecurityStamp),
            nameof(IdentityUser<TKey>.ConcurrencyStamp)
        };

        private static readonly HashSet<string> RoleCustomCopyExcludedPropertyNames = new(StringComparer.Ordinal)
        {
            nameof(IdentityRole<TKey>.Id),
            nameof(IdentityRole<TKey>.Name),
            nameof(IdentityRole<TKey>.NormalizedName),
            nameof(IdentityRole<TKey>.ConcurrencyStamp)
        };

        private static readonly HashSet<string> UserClaimCustomCopyExcludedPropertyNames = new(StringComparer.Ordinal)
        {
            nameof(IdentityUserClaim<TKey>.Id),
            nameof(IdentityUserClaim<TKey>.UserId),
            nameof(IdentityUserClaim<TKey>.ClaimType),
            nameof(IdentityUserClaim<TKey>.ClaimValue)
        };

        private static readonly HashSet<string> RoleClaimCustomCopyExcludedPropertyNames = new(StringComparer.Ordinal)
        {
            nameof(IdentityRoleClaim<TKey>.Id),
            nameof(IdentityRoleClaim<TKey>.RoleId),
            nameof(IdentityRoleClaim<TKey>.ClaimType),
            nameof(IdentityRoleClaim<TKey>.ClaimValue)
        };

        private static readonly HashSet<string> UserProviderCustomCopyExcludedPropertyNames = new(StringComparer.Ordinal)
        {
            nameof(UserLoginInfo.ProviderKey),
            nameof(UserLoginInfo.LoginProvider),
            nameof(UserLoginInfo.ProviderDisplayName),
            nameof(UserProviderDto<TKey>.UserId),
            nameof(UserProviderDto<TKey>.UserName)
        };
        private readonly IReadOnlyCollection<IIdentityUserMappingCustomizer<TUserDto, TUser>> _userMappingCustomizers;
        private readonly IReadOnlyCollection<IIdentityUserAuditSanitizer<TUserDto>> _userAuditSanitizers;
        private readonly IReadOnlyCollection<IIdentityRoleMappingCustomizer<TRoleDto, TRole>> _roleMappingCustomizers;
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>> ReadablePropertiesCache = new();
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> WritablePropertiesCache = new();

        public IdentityDataMapper(
            IEnumerable<IIdentityUserMappingCustomizer<TUserDto, TUser>> userMappingCustomizers = null,
            IEnumerable<IIdentityRoleMappingCustomizer<TRoleDto, TRole>> roleMappingCustomizers = null)
        {
            _userMappingCustomizers = userMappingCustomizers?.ToArray() ?? Array.Empty<IIdentityUserMappingCustomizer<TUserDto, TUser>>();
            _userAuditSanitizers = _userMappingCustomizers.OfType<IIdentityUserAuditSanitizer<TUserDto>>().ToArray();
            _roleMappingCustomizers = roleMappingCustomizers?.ToArray() ?? Array.Empty<IIdentityRoleMappingCustomizer<TRoleDto, TRole>>();
        }

        public TUsersDto MapPagedUsersToDto(PagedList<TUser> pagedUsers)
        {
            if (pagedUsers == null)
            {
                return null;
            }

            var usersDto = MapperInstanceFactory.CreateInstance<TUsersDto>();
            usersDto.TotalCount = pagedUsers.TotalCount;
            usersDto.PageSize = pagedUsers.PageSize;
            usersDto.Users = pagedUsers.Data?.Select(MapUserToDto).ToList() ?? [];

            return usersDto;
        }

        public TRolesDto MapPagedRolesToRolesDto(PagedList<TRole> pagedRoles)
        {
            if (pagedRoles == null)
            {
                return null;
            }

            var rolesDto = MapperInstanceFactory.CreateInstance<TRolesDto>();
            rolesDto.TotalCount = pagedRoles.TotalCount;
            rolesDto.PageSize = pagedRoles.PageSize;
            rolesDto.Roles = pagedRoles.Data?.Select(MapRoleToDto).ToList() ?? [];

            return rolesDto;
        }

        public TUserRolesDto MapPagedRolesToUserRolesDto(PagedList<TRole> pagedRoles)
        {
            if (pagedRoles == null)
            {
                return null;
            }

            var userRolesDto = MapperInstanceFactory.CreateInstance<TUserRolesDto>();
            userRolesDto.TotalCount = pagedRoles.TotalCount;
            userRolesDto.PageSize = pagedRoles.PageSize;
            userRolesDto.Roles = pagedRoles.Data?.Select(MapRoleToDto).ToList() ?? [];

            return userRolesDto;
        }

        public TUserClaimsDto MapPagedUserClaimsToDto(PagedList<TUserClaim> pagedClaims)
        {
            if (pagedClaims == null)
            {
                return null;
            }

            var userClaimsDto = MapperInstanceFactory.CreateInstance<TUserClaimsDto>();
            userClaimsDto.TotalCount = pagedClaims.TotalCount;
            userClaimsDto.PageSize = pagedClaims.PageSize;
            userClaimsDto.Claims = pagedClaims.Data?.Select(MapUserClaimToClaimDto).ToList() ?? [];

            return userClaimsDto;
        }

        public TRoleClaimsDto MapPagedRoleClaimsToDto(PagedList<TRoleClaim> pagedClaims)
        {
            if (pagedClaims == null)
            {
                return null;
            }

            var roleClaimsDto = MapperInstanceFactory.CreateInstance<TRoleClaimsDto>();
            roleClaimsDto.TotalCount = pagedClaims.TotalCount;
            roleClaimsDto.PageSize = pagedClaims.PageSize;
            roleClaimsDto.Claims = pagedClaims.Data?.Select(MapRoleClaimToClaimDto).ToList() ?? [];

            return roleClaimsDto;
        }

        public TUserDto SanitizeAuditUser(TUserDto user)
        {
            var sanitizedUser = AuditEventDataSanitizer.SanitizeUser(user);
            if (sanitizedUser == null)
            {
                return default;
            }

            ApplyUserAuditSanitizers(sanitizedUser);
            return sanitizedUser;
        }

        public TUsersDto SanitizeAuditUsers(TUsersDto users)
        {
            var sanitizedUsers = AuditEventDataSanitizer.SanitizeUsers<TUsersDto, TUserDto, TKey>(users);
            if (sanitizedUsers?.Users == null)
            {
                return sanitizedUsers;
            }

            foreach (var user in sanitizedUsers.Users)
            {
                if (user != null)
                {
                    ApplyUserAuditSanitizers(user);
                }
            }

            return sanitizedUsers;
        }

        public TUserDto MapUserToDto(TUser source)
        {
            var userDto = MapperInstanceFactory.CreateInstance<TUserDto>();
            userDto.Id = source.Id;
            userDto.UserName = source.UserName;
            userDto.Email = source.Email;
            userDto.EmailConfirmed = source.EmailConfirmed;
            userDto.PhoneNumber = source.PhoneNumber;
            userDto.PhoneNumberConfirmed = source.PhoneNumberConfirmed;
            userDto.LockoutEnabled = source.LockoutEnabled;
            userDto.TwoFactorEnabled = source.TwoFactorEnabled;
            userDto.AccessFailedCount = source.AccessFailedCount;
            userDto.LockoutEnd = source.LockoutEnd;
            CopyMatchingProperties(source, userDto, UserCustomCopyExcludedPropertyNames);
            ApplyUserEntityToDtoCustomizers(source, userDto);

            return userDto;
        }

        public TRoleDto MapRoleToDto(TRole source)
        {
            var roleDto = MapperInstanceFactory.CreateInstance<TRoleDto>();
            roleDto.Id = source.Id;
            roleDto.Name = source.Name;
            CopyMatchingProperties(source, roleDto, RoleCustomCopyExcludedPropertyNames);
            ApplyRoleEntityToDtoCustomizers(source, roleDto);

            return roleDto;
        }

        public TUserClaimsDto MapUserClaimToClaimsDto(TUserClaim source)
        {
            var claimDto = MapperInstanceFactory.CreateInstance<TUserClaimsDto>();
            claimDto.ClaimId = source.Id;
            claimDto.UserId = source.UserId;
            claimDto.ClaimType = source.ClaimType;
            claimDto.ClaimValue = source.ClaimValue;
            CopyMatchingProperties(source, claimDto, UserClaimCustomCopyExcludedPropertyNames);

            return claimDto;
        }

        public TRoleClaimsDto MapRoleClaimToRoleClaimsDto(TRoleClaim source)
        {
            var claimDto = MapperInstanceFactory.CreateInstance<TRoleClaimsDto>();
            claimDto.ClaimId = source.Id;
            claimDto.RoleId = source.RoleId;
            claimDto.ClaimType = source.ClaimType;
            claimDto.ClaimValue = source.ClaimValue;
            CopyMatchingProperties(source, claimDto, RoleClaimCustomCopyExcludedPropertyNames);

            return claimDto;
        }

        public TUserProviderDto MapUserLoginToProviderDto(TUserLogin source)
        {
            var providerDto = MapperInstanceFactory.CreateInstance<TUserProviderDto>();
            providerDto.UserId = source.UserId;
            providerDto.ProviderKey = source.ProviderKey;
            providerDto.LoginProvider = source.LoginProvider;
            providerDto.ProviderDisplayName = source.ProviderDisplayName;
            CopyMatchingProperties(source, providerDto, UserProviderCustomCopyExcludedPropertyNames);

            return providerDto;
        }

        public TUserProvidersDto MapUserLoginInfosToProvidersDto(List<UserLoginInfo> source)
        {
            var providersDto = MapperInstanceFactory.CreateInstance<TUserProvidersDto>();
            // Intentionally return a non-null container so callers can always access Providers safely.
            // This is asymmetric to MapPagedXxx null handling by design.
            providersDto.Providers = source?.Select(MapUserLoginInfoToProviderDto).ToList() ?? [];

            return providersDto;
        }

        public TUser MapUserDtoToEntity(TUserDto user)
        {
            var userEntity = MapperInstanceFactory.CreateInstance<TUser>();

            if (!user.IsDefaultId())
            {
                userEntity.Id = user.Id;
            }

            MapUserDtoToEntity(user, userEntity);

            return userEntity;
        }

        public TRole MapRoleDtoToEntity(TRoleDto role)
        {
            var roleEntity = MapperInstanceFactory.CreateInstance<TRole>();

            if (!role.IsDefaultId())
            {
                roleEntity.Id = role.Id;
            }

            MapRoleDtoToEntity(role, roleEntity);

            return roleEntity;
        }

        public void MapUserDtoToEntity(TUserDto source, TUser destination)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);

            destination.UserName = source.UserName;
            destination.Email = source.Email;
            destination.EmailConfirmed = source.EmailConfirmed;
            destination.PhoneNumber = source.PhoneNumber;
            destination.PhoneNumberConfirmed = source.PhoneNumberConfirmed;
            destination.LockoutEnabled = source.LockoutEnabled;
            destination.LockoutEnd = source.LockoutEnd;
            destination.TwoFactorEnabled = source.TwoFactorEnabled;
            destination.AccessFailedCount = source.AccessFailedCount;
            CopyMatchingProperties(source, destination, UserCustomCopyExcludedPropertyNames);
            ApplyUserDtoToEntityCustomizers(source, destination);
        }

        public void MapRoleDtoToEntity(TRoleDto source, TRole destination)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);

            destination.Name = source.Name;
            CopyMatchingProperties(source, destination, RoleCustomCopyExcludedPropertyNames);
            ApplyRoleDtoToEntityCustomizers(source, destination);
        }

        public TUserClaim MapUserClaimsDtoToEntity(TUserClaimsDto claimsDto)
        {
            var userClaim = MapperInstanceFactory.CreateInstance<TUserClaim>();
            userClaim.Id = claimsDto.ClaimId;
            userClaim.UserId = claimsDto.UserId;
            userClaim.ClaimType = claimsDto.ClaimType;
            userClaim.ClaimValue = claimsDto.ClaimValue;
            CopyMatchingProperties(claimsDto, userClaim, UserClaimCustomCopyExcludedPropertyNames);

            return userClaim;
        }

        public TRoleClaim MapRoleClaimsDtoToEntity(TRoleClaimsDto claimsDto)
        {
            var roleClaim = MapperInstanceFactory.CreateInstance<TRoleClaim>();
            roleClaim.Id = claimsDto.ClaimId;
            roleClaim.RoleId = claimsDto.RoleId;
            roleClaim.ClaimType = claimsDto.ClaimType;
            roleClaim.ClaimValue = claimsDto.ClaimValue;
            CopyMatchingProperties(claimsDto, roleClaim, RoleClaimCustomCopyExcludedPropertyNames);

            return roleClaim;
        }

        private TUserClaimDto MapUserClaimToClaimDto(TUserClaim source)
        {
            var claimDto = MapperInstanceFactory.CreateInstance<TUserClaimDto>();
            claimDto.ClaimId = source.Id;
            claimDto.UserId = source.UserId;
            claimDto.ClaimType = source.ClaimType;
            claimDto.ClaimValue = source.ClaimValue;
            CopyMatchingProperties(source, claimDto, UserClaimCustomCopyExcludedPropertyNames);

            return claimDto;
        }

        private TRoleClaimDto MapRoleClaimToClaimDto(TRoleClaim source)
        {
            var claimDto = MapperInstanceFactory.CreateInstance<TRoleClaimDto>();
            claimDto.ClaimId = source.Id;
            claimDto.RoleId = source.RoleId;
            claimDto.ClaimType = source.ClaimType;
            claimDto.ClaimValue = source.ClaimValue;
            CopyMatchingProperties(source, claimDto, RoleClaimCustomCopyExcludedPropertyNames);

            return claimDto;
        }

        private TUserProviderDto MapUserLoginInfoToProviderDto(UserLoginInfo source)
        {
            var providerDto = MapperInstanceFactory.CreateInstance<TUserProviderDto>();
            providerDto.ProviderKey = source.ProviderKey;
            providerDto.LoginProvider = source.LoginProvider;
            providerDto.ProviderDisplayName = source.ProviderDisplayName;
            CopyMatchingProperties(source, providerDto, UserProviderCustomCopyExcludedPropertyNames);

            return providerDto;
        }

        private void ApplyUserDtoToEntityCustomizers(TUserDto source, TUser destination)
        {
            foreach (var customizer in _userMappingCustomizers)
            {
                customizer.MapDtoToEntity(source, destination);
            }
        }

        private void ApplyUserEntityToDtoCustomizers(TUser source, TUserDto destination)
        {
            foreach (var customizer in _userMappingCustomizers)
            {
                customizer.MapEntityToDto(source, destination);
            }
        }

        private void ApplyUserAuditSanitizers(TUserDto user)
        {
            foreach (var sanitizer in _userAuditSanitizers)
            {
                sanitizer.SanitizeAuditUser(user);
            }
        }

        private void ApplyRoleDtoToEntityCustomizers(TRoleDto source, TRole destination)
        {
            foreach (var customizer in _roleMappingCustomizers)
            {
                customizer.MapDtoToEntity(source, destination);
            }
        }

        private void ApplyRoleEntityToDtoCustomizers(TRole source, TRoleDto destination)
        {
            foreach (var customizer in _roleMappingCustomizers)
            {
                customizer.MapEntityToDto(source, destination);
            }
        }

        private static void CopyMatchingProperties<TSource, TDestination>(TSource source, TDestination destination, IReadOnlySet<string> excludedProperties)
        {
            if (source == null || destination == null)
            {
                return;
            }

            var sourceProperties = GetReadablePropertiesMap(typeof(TSource));
            foreach (var destinationProperty in GetWritableProperties(typeof(TDestination)))
            {
                if (excludedProperties.Contains(destinationProperty.Name))
                {
                    continue;
                }

                if (!sourceProperties.TryGetValue(destinationProperty.Name, out var sourceProperty))
                {
                    continue;
                }

                // Intentional limitation: copy only assignable types; no implicit coercion (e.g. DateTime? <-> DateTime).
                if (!destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    continue;
                }

                destinationProperty.SetValue(destination, sourceProperty.GetValue(source));
            }
        }

        private static IReadOnlyDictionary<string, PropertyInfo> GetReadablePropertiesMap(Type type)
        {
            return ReadablePropertiesCache.GetOrAdd(type, targetType =>
                targetType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanRead && x.GetIndexParameters().Length == 0)
                    .OrderBy(x => GetInheritanceDistance(targetType, x.DeclaringType))
                    .GroupBy(x => x.Name, StringComparer.Ordinal)
                    .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal));
        }

        private static PropertyInfo[] GetWritableProperties(Type type)
        {
            return WritablePropertiesCache.GetOrAdd(type, targetType =>
                targetType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanWrite && x.GetIndexParameters().Length == 0)
                    .ToArray());
        }

        private static int GetInheritanceDistance(Type targetType, Type declaringType)
        {
            if (declaringType == null)
            {
                return int.MaxValue;
            }

            var distance = 0;
            for (var currentType = targetType; currentType != null; currentType = currentType.BaseType)
            {
                if (currentType == declaringType)
                {
                    return distance;
                }

                distance++;
            }

            return int.MaxValue;
        }
    }
}
