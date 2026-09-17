// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.AuditLogging.Services;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Events.Identity;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Helpers;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Resources;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Shared.Dtos.Common;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories.Interfaces;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Services
{
    public class IdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole,
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> : IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole,
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>
        where TUserDto : UserDto<TKey>
        where TRoleDto : RoleDto<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
    {
        protected readonly IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> IdentityRepository;
        protected readonly IIdentityServiceResources IdentityServiceResources;
        protected readonly IAuditEventLogger AuditEventLogger;
        protected readonly IIdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto> IdentityDataMapper;

        public IdentityService(IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken> identityRepository,
            IIdentityServiceResources identityServiceResources,
            IAuditEventLogger auditEventLogger,
            IIdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
                TUserClaimDto, TRoleClaimDto> identityDataMapper)
        {
            IdentityRepository = identityRepository;
            IdentityServiceResources = identityServiceResources;
            AuditEventLogger = auditEventLogger;
            IdentityDataMapper = identityDataMapper;
        }

        private TUserDto SanitizeAuditUser(TUserDto user)
        {
            return IdentityDataMapper is IIdentityAuditDataMapper<TUserDto, TUsersDto> auditDataMapper
                ? auditDataMapper.SanitizeAuditUser(user)
                : AuditEventDataSanitizer.SanitizeUser(user);
        }

        private TUsersDto SanitizeAuditUsers(TUsersDto users)
        {
            return IdentityDataMapper is IIdentityAuditDataMapper<TUserDto, TUsersDto> auditDataMapper
                ? auditDataMapper.SanitizeAuditUsers(users)
                : AuditEventDataSanitizer.SanitizeUsers<TUsersDto, TUserDto, TKey>(users);
        }

        public virtual async Task<bool> ExistsUserAsync(string userId)
        {
            var exists = await IdentityRepository.ExistsUserAsync(userId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            return true;
        }

        public virtual async Task<bool> ExistsRoleAsync(string roleId)
        {
            var exists = await IdentityRepository.ExistsRoleAsync(roleId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            return true;
        }

        public virtual async Task<TUsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await IdentityRepository.GetUsersAsync(search, page, pageSize);
            var usersDto = IdentityDataMapper.MapPagedUsersToDto(pagedList);

            await AuditEventLogger.LogEventAsync(new UsersRequestedEvent<TUsersDto>(SanitizeAuditUsers(usersDto)));

            return usersDto;
        }

        public virtual async Task<TUsersDto> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10)
        {
            var roleKey = ConvertToKeyFromString(roleId);

            var userIdentityRole = await IdentityRepository.GetRoleAsync(roleKey);
            if (userIdentityRole == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var pagedList = await IdentityRepository.GetRoleUsersAsync(roleId, search, page, pageSize);
            var usersDto = IdentityDataMapper.MapPagedUsersToDto(pagedList);

            await AuditEventLogger.LogEventAsync(new RoleUsersRequestedEvent<TUsersDto>(SanitizeAuditUsers(usersDto)));

            return usersDto;
        }

        public virtual async Task<TUsersDto> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10)
        {
            var pagedList = await IdentityRepository.GetClaimUsersAsync(claimType, claimValue, page, pageSize);
            var usersDto = IdentityDataMapper.MapPagedUsersToDto(pagedList);

            await AuditEventLogger.LogEventAsync(new ClaimUsersRequestedEvent<TUsersDto>(SanitizeAuditUsers(usersDto)));

            return usersDto;
        }
        public virtual async Task<TRolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            PagedList<TRole> pagedList = await IdentityRepository.GetRolesAsync(search, page, pageSize);
            var rolesDto = IdentityDataMapper.MapPagedRolesToRolesDto(pagedList);

            await AuditEventLogger.LogEventAsync(new RolesRequestedEvent<TRolesDto>(rolesDto));

            return rolesDto;
        }

        public virtual async Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRoleDto role)
        {
            var roleEntity = IdentityDataMapper.MapRoleDtoToEntity(role);
            var (identityResult, roleId) = await IdentityRepository.CreateRoleAsync(roleEntity);
            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.RoleCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);

            await AuditEventLogger.LogEventAsync(new RoleAddedEvent<TRoleDto>(role));

            return (handleIdentityError, roleId);
        }

        private IdentityResult HandleIdentityError(IdentityResult identityResult, string errorMessage, string errorKey, object model)
        {
            if (!identityResult.Errors.Any()) return identityResult;
            var viewErrorMessages = identityResult.Errors
                .Select(error => new ViewErrorMessage
                {
                    ErrorKey = error.Code,
                    ErrorMessage = error.Description
                })
                .ToList();

            throw new UserFriendlyViewException(errorMessage, errorKey, viewErrorMessages, model);
        }

        public virtual async Task<TRoleDto> GetRoleAsync(string roleId)
        {
            var roleKey = ConvertToKeyFromString(roleId);

            var userIdentityRole = await IdentityRepository.GetRoleAsync(roleKey);
            if (userIdentityRole == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var roleDto = IdentityDataMapper.MapRoleToDto(userIdentityRole);

            await AuditEventLogger.LogEventAsync(new RoleRequestedEvent<TRoleDto>(roleDto));

            return roleDto;
        }

        public virtual async Task<List<TRoleDto>> GetRolesAsync()
        {
            var roles = await IdentityRepository.GetRolesAsync();
            var roleDtos = roles.Select(IdentityDataMapper.MapRoleToDto).ToList();

            await AuditEventLogger.LogEventAsync(new AllRolesRequestedEvent<TRoleDto>(roleDtos));

            return roleDtos;
        }

        public virtual async Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRoleDto role)
        {
            var originalRole = await GetRoleAsync(role.Id.ToString());
            var roleEntity = await IdentityRepository.GetRoleAsync(role.Id);
            if (roleEntity == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, role.Id), IdentityServiceResources.RoleDoesNotExist().Description);

            IdentityDataMapper.MapRoleDtoToEntity(role, roleEntity);

            var (identityResult, roleId) = await IdentityRepository.UpdateRoleAsync(roleEntity);

            await AuditEventLogger.LogEventAsync(new RoleUpdatedEvent<TRoleDto>(originalRole, role));

            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.RoleUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);

            return (handleIdentityError, roleId);
        }

        public virtual async Task<TUserDto> GetUserAsync(string userId)
        {
            var identity = await IdentityRepository.GetUserAsync(userId);
            if (identity == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var userDto = IdentityDataMapper.MapUserToDto(identity);

            await AuditEventLogger.LogEventAsync(new UserRequestedEvent<TUserDto>(SanitizeAuditUser(userDto)));

            return userDto;
        }

        public virtual async Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUserDto user)
        {
            var userIdentity = IdentityDataMapper.MapUserDtoToEntity(user);
            var (identityResult, userId) = await IdentityRepository.CreateUserAsync(userIdentity);

            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.UserCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, user);

            await AuditEventLogger.LogEventAsync(new UserSavedEvent<TUserDto>(SanitizeAuditUser(user)));

            return (handleIdentityError, userId);
        }

        /// <summary>
        /// Updates the specified user, but without updating the password hash value
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUserDto user)
        {
            var originalUser = await GetUserAsync(user.Id.ToString());
            var userIdentity = await IdentityRepository.GetUserAsync(user.Id.ToString());
            if (userIdentity == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, user.Id), IdentityServiceResources.UserDoesNotExist().Description);

            IdentityDataMapper.MapUserDtoToEntity(user, userIdentity);

            var (identityResult, userId) = await IdentityRepository.UpdateUserAsync(userIdentity);
            var handleIdentityError = HandleIdentityError(identityResult, IdentityServiceResources.UserUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, user);

            await AuditEventLogger.LogEventAsync(new UserUpdatedEvent<TUserDto>(
                SanitizeAuditUser(originalUser),
                SanitizeAuditUser(user)));

            return (handleIdentityError, userId);
        }

        public virtual async Task<IdentityResult> DeleteUserAsync(string userId, TUserDto user)
        {
            var identityResult = await IdentityRepository.DeleteUserAsync(userId);

            await AuditEventLogger.LogEventAsync(new UserDeletedEvent<TUserDto>(SanitizeAuditUser(user)));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, user);
        }

        public virtual async Task<IdentityResult> CreateUserRoleAsync(TUserRolesDto role)
        {
            var identityResult = await IdentityRepository.CreateUserRoleAsync(role.UserId.ToString(), role.RoleId.ToString());

            await AuditEventLogger.LogEventAsync(new UserRoleSavedEvent<TUserRolesDto>(role));

            if (!identityResult.Errors.Any()) return identityResult;

            var userRolesDto = await BuildUserRolesViewModel(role.UserId, 1);

            return HandleIdentityError(identityResult, IdentityServiceResources.UserRoleCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, userRolesDto);
        }

        public virtual async Task<TUserRolesDto> BuildUserRolesViewModel(TKey id, int? page)
        {
            var roles = await GetRolesAsync();
            var userRoles = await GetUserRolesAsync(id.ToString(), page ?? 1);
            userRoles.UserId = id;
            userRoles.RolesList = roles.Select(x => new SelectItemDto(x.Id.ToString(), x.Name)).ToList();

            return userRoles;
        }

        public virtual async Task<TUserRolesDto> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var userIdentityRoles = await IdentityRepository.GetUserRolesAsync(userId, page, pageSize);
            var roleDtos = IdentityDataMapper.MapPagedRolesToUserRolesDto(userIdentityRoles);

            var user = await IdentityRepository.GetUserAsync(userId);
            roleDtos.UserName = user.UserName;

            await AuditEventLogger.LogEventAsync(new UserRolesRequestedEvent<TUserRolesDto>(roleDtos));

            return roleDtos;
        }

        public virtual async Task<IdentityResult> DeleteUserRoleAsync(TUserRolesDto role)
        {
            var identityResult = await IdentityRepository.DeleteUserRoleAsync(role.UserId.ToString(), role.RoleId.ToString());

            await AuditEventLogger.LogEventAsync(new UserRoleDeletedEvent<TUserRolesDto>(role));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserRoleDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);
        }

        public virtual async Task<TUserClaimsDto> GetUserClaimsAsync(string userId, int page = 1, int pageSize = 10)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityUserClaims = await IdentityRepository.GetUserClaimsAsync(userId, page, pageSize);
            var claimDtos = IdentityDataMapper.MapPagedUserClaimsToDto(identityUserClaims);

            var user = await IdentityRepository.GetUserAsync(userId);
            claimDtos.UserName = user.UserName;

            await AuditEventLogger.LogEventAsync(new UserClaimsRequestedEvent<TUserClaimsDto>(claimDtos));

            return claimDtos;
        }

        public virtual async Task<TUserClaimsDto> GetUserClaimAsync(string userId, int claimId)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityUserClaim = await IdentityRepository.GetUserClaimAsync(userId, claimId);
            if (identityUserClaim == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserClaimDoesNotExist().Description, userId), IdentityServiceResources.UserClaimDoesNotExist().Description);

            var userClaimsDto = IdentityDataMapper.MapUserClaimToClaimsDto(identityUserClaim);

            await AuditEventLogger.LogEventAsync(new UserClaimRequestedEvent<TUserClaimsDto>(userClaimsDto));

            return userClaimsDto;
        }

        public virtual async Task<IdentityResult> CreateUserClaimsAsync(TUserClaimsDto claimsDto)
        {
            var userIdentityUserClaim = IdentityDataMapper.MapUserClaimsDtoToEntity(claimsDto);
            var identityResult = await IdentityRepository.CreateUserClaimsAsync(userIdentityUserClaim);

            await AuditEventLogger.LogEventAsync(new UserClaimsSavedEvent<TUserClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserClaimsCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<IdentityResult> UpdateUserClaimsAsync(TUserClaimsDto claimsDto)
        {
            var userIdentityUserClaim = IdentityDataMapper.MapUserClaimsDtoToEntity(claimsDto);
            var identityResult = await IdentityRepository.UpdateUserClaimsAsync(userIdentityUserClaim);

            await AuditEventLogger.LogEventAsync(new UserClaimsSavedEvent<TUserClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserClaimsUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<IdentityResult> DeleteUserClaimAsync(TUserClaimsDto claim)
        {
            var deleted = await IdentityRepository.DeleteUserClaimAsync(claim.UserId.ToString(), claim.ClaimId);

            await AuditEventLogger.LogEventAsync(new UserClaimsDeletedEvent<TUserClaimsDto>(claim));

            return deleted;
        }

        public virtual TKey ConvertToKeyFromString(string id)
        {
            if (id == null)
            {
                return default(TKey);
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        public virtual async Task<TUserProvidersDto> GetUserProvidersAsync(string userId)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var userLoginInfos = await IdentityRepository.GetUserProvidersAsync(userId);
            var providersDto = IdentityDataMapper.MapUserLoginInfosToProvidersDto(userLoginInfos);
            providersDto.UserId = ConvertToKeyFromString(userId);

            var user = await IdentityRepository.GetUserAsync(userId);
            providersDto.UserName = user.UserName;

            await AuditEventLogger.LogEventAsync(new UserProvidersRequestedEvent<TUserProvidersDto>(providersDto));

            return providersDto;
        }

        public virtual async Task<IdentityResult> DeleteUserProvidersAsync(TUserProviderDto provider)
        {
            var identityResult = await IdentityRepository.DeleteUserProvidersAsync(provider.UserId.ToString(), provider.ProviderKey, provider.LoginProvider);

            await AuditEventLogger.LogEventAsync(new UserProvidersDeletedEvent<TUserProviderDto>(provider));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserProviderDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, provider);
        }

        public virtual async Task<TUserProviderDto> GetUserProviderAsync(string userId, string providerKey)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityUserLogin = await IdentityRepository.GetUserProviderAsync(userId, providerKey);
            if (identityUserLogin == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserProviderDoesNotExist().Description, providerKey), IdentityServiceResources.UserProviderDoesNotExist().Description);

            var userProviderDto = IdentityDataMapper.MapUserLoginToProviderDto(identityUserLogin);
            var user = await GetUserAsync(userId);
            userProviderDto.UserName = user.UserName;

            await AuditEventLogger.LogEventAsync(new UserProviderRequestedEvent<TUserProviderDto>(userProviderDto));

            return userProviderDto;
        }

        public virtual async Task<IdentityResult> UserChangePasswordAsync(TUserChangePasswordDto userPassword)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userPassword.UserId.ToString());
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userPassword.UserId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityResult = await IdentityRepository.UserChangePasswordAsync(userPassword.UserId.ToString(), userPassword.Password);

            await AuditEventLogger.LogEventAsync(new UserPasswordChangedEvent(userPassword.UserName));

            return HandleIdentityError(identityResult, IdentityServiceResources.UserChangePasswordFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, userPassword);
        }

        public virtual async Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaimsDto claimsDto)
        {
            var identityRoleClaim = IdentityDataMapper.MapRoleClaimsDtoToEntity(claimsDto);
            var identityResult = await IdentityRepository.CreateRoleClaimsAsync(identityRoleClaim);

            await AuditEventLogger.LogEventAsync(new RoleClaimsSavedEvent<TRoleClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.RoleClaimsCreateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<IdentityResult> UpdateRoleClaimsAsync(TRoleClaimsDto claimsDto)
        {
            var identityRoleClaim = IdentityDataMapper.MapRoleClaimsDtoToEntity(claimsDto);
            var identityResult = await IdentityRepository.UpdateRoleClaimsAsync(identityRoleClaim);

            await AuditEventLogger.LogEventAsync(new RoleClaimsSavedEvent<TRoleClaimsDto>(claimsDto));

            return HandleIdentityError(identityResult, IdentityServiceResources.RoleClaimsUpdateFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, claimsDto);
        }

        public virtual async Task<TRoleClaimsDto> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10)
        {
            var roleExists = await IdentityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaims = await IdentityRepository.GetRoleClaimsAsync(roleId, page, pageSize);
            var roleClaimDtos = IdentityDataMapper.MapPagedRoleClaimsToDto(identityRoleClaims);
            var roleDto = await GetRoleAsync(roleId);
            roleClaimDtos.RoleName = roleDto.Name;

            await AuditEventLogger.LogEventAsync(new RoleClaimsRequestedEvent<TRoleClaimsDto>(roleClaimDtos));

            return roleClaimDtos;
        }

        public virtual async Task<TRoleClaimsDto> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10)
        {
            var userExists = await IdentityRepository.ExistsUserAsync(userId);
            if (!userExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.UserDoesNotExist().Description, userId), IdentityServiceResources.UserDoesNotExist().Description);

            var identityRoleClaims = await IdentityRepository.GetUserRoleClaimsAsync(userId, claimSearchText, page, pageSize);
            var roleClaimDtos = IdentityDataMapper.MapPagedRoleClaimsToDto(identityRoleClaims);

            return roleClaimDtos;
        }

        public virtual async Task<TRoleClaimsDto> GetRoleClaimAsync(string roleId, int claimId)
        {
            var roleExists = await IdentityRepository.ExistsRoleAsync(roleId);
            if (!roleExists) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleDoesNotExist().Description, roleId), IdentityServiceResources.RoleDoesNotExist().Description);

            var identityRoleClaim = await IdentityRepository.GetRoleClaimAsync(roleId, claimId);
            if (identityRoleClaim == null) throw new UserFriendlyErrorPageException(string.Format(IdentityServiceResources.RoleClaimDoesNotExist().Description, claimId), IdentityServiceResources.RoleClaimDoesNotExist().Description);
            var roleClaimsDto = IdentityDataMapper.MapRoleClaimToRoleClaimsDto(identityRoleClaim);
            var roleDto = await GetRoleAsync(roleId);
            roleClaimsDto.RoleName = roleDto.Name;

            await AuditEventLogger.LogEventAsync(new RoleClaimRequestedEvent<TRoleClaimsDto>(roleClaimsDto));

            return roleClaimsDto;
        }

        public virtual async Task<IdentityResult> DeleteRoleClaimAsync(TRoleClaimsDto role)
        {
            var deleted = await IdentityRepository.DeleteRoleClaimAsync(role.RoleId.ToString(), role.ClaimId);

            await AuditEventLogger.LogEventAsync(new RoleClaimsDeletedEvent<TRoleClaimsDto>(role));

            return deleted;
        }

        public virtual async Task<IdentityResult> DeleteRoleAsync(TRoleDto role)
        {
            var userIdentityRole = IdentityDataMapper.MapRoleDtoToEntity(role);
            var identityResult = await IdentityRepository.DeleteRoleAsync(userIdentityRole);

            await AuditEventLogger.LogEventAsync(new RoleDeletedEvent<TRoleDto>(role));

            return HandleIdentityError(identityResult, IdentityServiceResources.RoleDeleteFailed().Description, IdentityServiceResources.IdentityErrorKey().Description, role);
        }

    }
}
