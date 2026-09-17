// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Roles;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Users;
using Toralux.Open.IdentityServer.Admin.UI.Api.Mappers;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mappers
{
    public class IdentityApiMappers
    {
        [Fact]
        public void CanMapUserRolesDtoToUserRolesApiDto()
        {
            var userRolesDto = IdentityDtoMock<string>.GenerateRandomUserRole<RoleDto<string>>(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            userRolesDto.PageSize = 10;
            userRolesDto.TotalCount = 20;
            userRolesDto.Roles.Add(IdentityDtoMock<string>.GenerateRandomRole(userRolesDto.RoleId));

            var userRolesApiDto = userRolesDto.ToUserRolesApiDto<RoleDto<string>, string>();

            userRolesApiDto.Roles.Should().BeEquivalentTo(userRolesDto.Roles);
            userRolesApiDto.PageSize.Should().Be(userRolesDto.PageSize);
            userRolesApiDto.TotalCount.Should().Be(userRolesDto.TotalCount);
        }

        [Fact]
        public void CanMapUserRoleApiDtoToUserRolesDto()
        {
            var userRoleApiDto = new UserRoleApiDto<string>
            {
                UserId = Guid.NewGuid().ToString(),
                RoleId = Guid.NewGuid().ToString()
            };

            var userRolesDto = userRoleApiDto.ToUserRolesDto<UserRolesDto<RoleDto<string>, string>, string>();

            userRolesDto.UserId.Should().Be(userRoleApiDto.UserId);
            userRolesDto.RoleId.Should().Be(userRoleApiDto.RoleId);
        }

        [Fact]
        public void CanMapUserClaimsDtoToUserClaimsApiDto()
        {
            var userClaimsDto = IdentityDtoMock<string>.GenerateRandomUserClaim(1, Guid.NewGuid().ToString());
            userClaimsDto.PageSize = 5;
            userClaimsDto.TotalCount = 15;
            userClaimsDto.Claims.Add(new UserClaimDto<string>
            {
                ClaimId = userClaimsDto.ClaimId,
                UserId = userClaimsDto.UserId,
                ClaimType = userClaimsDto.ClaimType,
                ClaimValue = userClaimsDto.ClaimValue
            });

            var userClaimsApiDto = userClaimsDto.ToUserClaimsApiDto<UserClaimDto<string>, string>();

            userClaimsApiDto.Claims.Should().BeEquivalentTo(userClaimsDto.Claims);
            userClaimsApiDto.PageSize.Should().Be(userClaimsDto.PageSize);
            userClaimsApiDto.TotalCount.Should().Be(userClaimsDto.TotalCount);
        }

        [Fact]
        public void CanMapUserClaimApiDtoToUserClaimsDto()
        {
            var userClaimApiDto = new UserClaimApiDto<string>
            {
                ClaimId = 1,
                UserId = Guid.NewGuid().ToString(),
                ClaimType = Guid.NewGuid().ToString(),
                ClaimValue = Guid.NewGuid().ToString()
            };

            var userClaimsDto = userClaimApiDto.ToUserClaimsDto<UserClaimsDto<UserClaimDto<string>, string>, string>();

            userClaimsDto.Should().BeEquivalentTo(userClaimApiDto);
        }

        [Fact]
        public void CanMapUserProvidersDtoToUserProvidersApiDto()
        {
            var userProvidersDto = new UserProvidersDto<UserProviderDto<string>, string>
            {
                UserId = Guid.NewGuid().ToString()
            };

            userProvidersDto.Providers.Add(new UserProviderDto<string>
            {
                UserId = userProvidersDto.UserId,
                UserName = Guid.NewGuid().ToString(),
                ProviderKey = Guid.NewGuid().ToString(),
                LoginProvider = Guid.NewGuid().ToString(),
                ProviderDisplayName = Guid.NewGuid().ToString()
            });

            var userProvidersApiDto = userProvidersDto.ToUserProvidersApiDto<UserProviderDto<string>, string>();

            userProvidersApiDto.Providers.Should().BeEquivalentTo(userProvidersDto.Providers);
        }

        [Fact]
        public void CanMapUserProviderDeleteApiDtoToUserProviderDto()
        {
            var userProviderDeleteApiDto = new UserProviderDeleteApiDto<string>
            {
                UserId = Guid.NewGuid().ToString(),
                ProviderKey = Guid.NewGuid().ToString(),
                LoginProvider = Guid.NewGuid().ToString()
            };

            var userProviderDto = userProviderDeleteApiDto.ToUserProviderDto<UserProviderDto<string>, string>();

            userProviderDto.Should().BeEquivalentTo(userProviderDeleteApiDto);
        }

        [Fact]
        public void CanMapUserChangePasswordApiDtoToUserChangePasswordDto()
        {
            var password = Guid.NewGuid().ToString("N");
            var userChangePasswordApiDto = new UserChangePasswordApiDto<string>
            {
                UserId = Guid.NewGuid().ToString(),
                Password = password,
                ConfirmPassword = password
            };

            var userChangePasswordDto = userChangePasswordApiDto.ToUserChangePasswordDto<UserChangePasswordDto<string>, string>();

            userChangePasswordDto.Should().BeEquivalentTo(userChangePasswordApiDto);
        }

        [Fact]
        public void CanMapRoleClaimsDtoToRoleClaimsApiDto()
        {
            var roleClaimsDto = IdentityDtoMock<string>.GenerateRandomRoleClaim(1, Guid.NewGuid().ToString());
            roleClaimsDto.PageSize = 7;
            roleClaimsDto.TotalCount = 17;
            roleClaimsDto.Claims.Add(new RoleClaimDto<string>
            {
                ClaimId = roleClaimsDto.ClaimId,
                RoleId = roleClaimsDto.RoleId,
                ClaimType = roleClaimsDto.ClaimType,
                ClaimValue = roleClaimsDto.ClaimValue
            });

            var roleClaimsApiDto = roleClaimsDto.ToRoleClaimsApiDto<RoleClaimDto<string>, string>();

            roleClaimsApiDto.Claims.Should().BeEquivalentTo(roleClaimsDto.Claims);
            roleClaimsApiDto.PageSize.Should().Be(roleClaimsDto.PageSize);
            roleClaimsApiDto.TotalCount.Should().Be(roleClaimsDto.TotalCount);
        }

        [Fact]
        public void CanMapRoleClaimApiDtoToRoleClaimsDto()
        {
            var roleClaimApiDto = new RoleClaimApiDto<string>
            {
                ClaimId = 1,
                RoleId = Guid.NewGuid().ToString(),
                ClaimType = Guid.NewGuid().ToString(),
                ClaimValue = Guid.NewGuid().ToString()
            };

            var roleClaimsDto = roleClaimApiDto.ToRoleClaimsDto<RoleClaimsDto<RoleClaimDto<string>, string>, string>();

            roleClaimsDto.Should().BeEquivalentTo(roleClaimApiDto);
        }
    }
}
