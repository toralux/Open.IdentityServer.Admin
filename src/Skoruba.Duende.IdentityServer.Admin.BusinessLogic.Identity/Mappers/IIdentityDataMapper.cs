// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers
{
    public interface IIdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
        TUserClaimDto, TRoleClaimDto>
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
        TUsersDto MapPagedUsersToDto(PagedList<TUser> pagedUsers);
        TRolesDto MapPagedRolesToRolesDto(PagedList<TRole> pagedRoles);
        TUserRolesDto MapPagedRolesToUserRolesDto(PagedList<TRole> pagedRoles);
        TUserClaimsDto MapPagedUserClaimsToDto(PagedList<TUserClaim> pagedClaims);
        TRoleClaimsDto MapPagedRoleClaimsToDto(PagedList<TRoleClaim> pagedClaims);

        TUserDto MapUserToDto(TUser source);
        TRoleDto MapRoleToDto(TRole source);
        TUserClaimsDto MapUserClaimToClaimsDto(TUserClaim source);
        TRoleClaimsDto MapRoleClaimToRoleClaimsDto(TRoleClaim source);
        TUserProviderDto MapUserLoginToProviderDto(TUserLogin source);
        TUserProvidersDto MapUserLoginInfosToProvidersDto(List<UserLoginInfo> source);

        TUser MapUserDtoToEntity(TUserDto user);
        TRole MapRoleDtoToEntity(TRoleDto role);
        void MapUserDtoToEntity(TUserDto source, TUser destination);
        void MapRoleDtoToEntity(TRoleDto source, TRole destination);
        TUserClaim MapUserClaimsDtoToEntity(TUserClaimsDto claimsDto);
        TRoleClaim MapRoleClaimsDtoToEntity(TRoleClaimsDto claimsDto);
    }
}
