// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity.Base;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Roles;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Users;

namespace Toralux.Open.IdentityServer.Admin.UI.Api.Mappers
{
    public static class IdentityApiMappers
    {
        public static UserRolesApiDto<TRoleDto> ToUserRolesApiDto<TRoleDto, TKey>(this UserRolesDto<TRoleDto, TKey> source)
            where TRoleDto : RoleDto<TKey>
        {
            return new UserRolesApiDto<TRoleDto>
            {
                Roles = source?.Roles ?? [],
                PageSize = source?.PageSize ?? default,
                TotalCount = source?.TotalCount ?? default
            };
        }

        public static TUserRolesDto ToUserRolesDto<TUserRolesDto, TKey>(this UserRoleApiDto<TKey> source)
            where TUserRolesDto : BaseUserRolesDto<TKey>
        {
            var dto = MapperInstanceFactory.CreateInstance<TUserRolesDto>();
            dto.UserId = source.UserId;
            dto.RoleId = source.RoleId;
            return dto;
        }

        public static UserClaimsApiDto<TKey> ToUserClaimsApiDto<TUserClaimDto, TKey>(this UserClaimsDto<TUserClaimDto, TKey> source)
            where TUserClaimDto : UserClaimDto<TKey>
        {
            return new UserClaimsApiDto<TKey>
            {
                Claims = source?.Claims?.Select(ToUserClaimApiDto).ToList() ?? [],
                PageSize = source?.PageSize ?? default,
                TotalCount = source?.TotalCount ?? default
            };
        }

        public static TUserClaimsDto ToUserClaimsDto<TUserClaimsDto, TKey>(this UserClaimApiDto<TKey> source)
            where TUserClaimsDto : UserClaimDto<TKey>
        {
            var dto = MapperInstanceFactory.CreateInstance<TUserClaimsDto>();
            dto.ClaimId = source.ClaimId;
            dto.UserId = source.UserId;
            dto.ClaimType = source.ClaimType;
            dto.ClaimValue = source.ClaimValue;
            return dto;
        }

        public static UserProvidersApiDto<TKey> ToUserProvidersApiDto<TUserProviderDto, TKey>(this UserProvidersDto<TUserProviderDto, TKey> source)
            where TUserProviderDto : UserProviderDto<TKey>
        {
            return new UserProvidersApiDto<TKey>
            {
                Providers = source?.Providers?.Select(ToUserProviderApiDto).ToList() ?? []
            };
        }

        public static TUserProviderDto ToUserProviderDto<TUserProviderDto, TKey>(this UserProviderDeleteApiDto<TKey> source)
            where TUserProviderDto : UserProviderDto<TKey>
        {
            var dto = MapperInstanceFactory.CreateInstance<TUserProviderDto>();
            dto.UserId = source.UserId;
            dto.ProviderKey = source.ProviderKey;
            dto.LoginProvider = source.LoginProvider;
            return dto;
        }

        public static TUserChangePasswordDto ToUserChangePasswordDto<TUserChangePasswordDto, TKey>(this UserChangePasswordApiDto<TKey> source)
            where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        {
            var dto = MapperInstanceFactory.CreateInstance<TUserChangePasswordDto>();
            dto.UserId = source.UserId;
            dto.Password = source.Password;
            dto.ConfirmPassword = source.ConfirmPassword;
            return dto;
        }

        public static RoleClaimsApiDto<TKey> ToRoleClaimsApiDto<TRoleClaimDto, TKey>(this RoleClaimsDto<TRoleClaimDto, TKey> source)
            where TRoleClaimDto : RoleClaimDto<TKey>
        {
            return new RoleClaimsApiDto<TKey>
            {
                Claims = source?.Claims?.Select(ToRoleClaimApiDto).ToList() ?? [],
                PageSize = source?.PageSize ?? default,
                TotalCount = source?.TotalCount ?? default
            };
        }

        public static TRoleClaimsDto ToRoleClaimsDto<TRoleClaimsDto, TKey>(this RoleClaimApiDto<TKey> source)
            where TRoleClaimsDto : RoleClaimDto<TKey>
        {
            var dto = MapperInstanceFactory.CreateInstance<TRoleClaimsDto>();
            dto.ClaimId = source.ClaimId;
            dto.RoleId = source.RoleId;
            dto.ClaimType = source.ClaimType;
            dto.ClaimValue = source.ClaimValue;
            return dto;
        }

        private static UserClaimApiDto<TKey> ToUserClaimApiDto<TKey>(UserClaimDto<TKey> source)
        {
            return new UserClaimApiDto<TKey>
            {
                ClaimId = source.ClaimId,
                UserId = source.UserId,
                ClaimType = source.ClaimType,
                ClaimValue = source.ClaimValue
            };
        }

        private static UserProviderApiDto<TKey> ToUserProviderApiDto<TKey>(UserProviderDto<TKey> source)
        {
            return new UserProviderApiDto<TKey>
            {
                UserId = source.UserId,
                UserName = source.UserName,
                ProviderKey = source.ProviderKey,
                LoginProvider = source.LoginProvider,
                ProviderDisplayName = source.ProviderDisplayName
            };
        }

        private static RoleClaimApiDto<TKey> ToRoleClaimApiDto<TKey>(RoleClaimDto<TKey> source)
        {
            return new RoleClaimApiDto<TKey>
            {
                ClaimId = source.ClaimId,
                RoleId = source.RoleId,
                ClaimType = source.ClaimType,
                ClaimValue = source.ClaimValue
            };
        }

    }
}
