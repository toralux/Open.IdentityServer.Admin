// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.Api.IntegrationTests.Tests.Base;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Roles;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Users;
using Toralux.Open.IdentityServer.Shared.Dtos.Identity;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.Api.IntegrationTests.Tests
{
    public class RolesControllerTests : AdminApiTestBase
    {
        private const string RoleSearchParameter = "searchText";
        private const string RolesUsersRouteSuffix = "users";
        private const string RolesClaimsRouteSuffix = "claims";
        private const string UpdatedSuffix = "_updated";
        private const string RoleIdPrefix = "role_id";
        private const string ClaimTypePrefix = "role_claim_type";
        private const string ClaimValuePrefix = "role_claim_value";
        private const int NonDefaultClaimId = 1;

        public RolesControllerTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetRolesAsAdmin()
        {
            SetupAdminAuthorization();

            var response = await Client.GetAsync(RolesRoute);

            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var roles = await response.Content.ReadFromJsonAsync<IdentityRolesDto>();
            roles.Should().NotBeNull();
            roles!.Roles.Should().NotBeNull();
            roles.TotalCount.Should().BeGreaterOrEqualTo(0);
        }

        [Fact]
        public async Task GetRolesWithoutPermissions()
        {
            ClearAuthorization();

            var response = await Client.GetAsync(RolesRoute);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RoleCreateWithoutPermissionsReturnsUnauthorized()
        {
            ClearAuthorization();
            var createRequest = RoleCreateFaker.Generate();

            var response = await Client.PostAsJsonAsync(RolesRoute, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RoleCreateWithExplicitIdReturnsBadRequest()
        {
            SetupAdminAuthorization();
            var createRequest = RoleCreateFaker.Generate();
            createRequest.Id = UniqueValue(RoleIdPrefix);

            var response = await Client.PostAsJsonAsync(RolesRoute, createRequest);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetRolesSupportsSearchByName()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var response = await Client.GetAsync(BuildSearchQuery(RolesRoute, RoleSearchParameter, roleName));
                response.EnsureSuccessStatusCode();

                var roles = await response.Content.ReadFromJsonAsync<IdentityRolesDto>();
                roles.Should().NotBeNull();
                roles!.Roles.Should().Contain(x => x.Id == createdRole.Id && x.Name == roleName);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task GetRoleByIdReturnsCreatedRole()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var response = await Client.GetAsync(ById(RolesRoute, createdRoleId));
                response.EnsureSuccessStatusCode();

                var role = await response.Content.ReadFromJsonAsync<IdentityRoleDto>();
                role.Should().NotBeNull();
                role!.Id.Should().Be(createdRoleId);
                role.Name.Should().Be(roleName);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task RoleCreateUpdateDeleteRoundTripWorks()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            var updatedRoleName = $"{roleName}{UpdatedSuffix}";
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var detailResponse = await Client.GetAsync(ById(RolesRoute, createdRoleId));
                detailResponse.EnsureSuccessStatusCode();
                var roleDetail = await detailResponse.Content.ReadFromJsonAsync<IdentityRoleDto>();
                roleDetail.Should().NotBeNull();
                roleDetail!.Name.Should().Be(roleName);

                roleDetail.Name = updatedRoleName;
                var updateResponse = await Client.PutAsJsonAsync(RolesRoute, roleDetail);
                updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                var updatedResponse = await Client.GetAsync(ById(RolesRoute, createdRoleId));
                updatedResponse.EnsureSuccessStatusCode();
                var updatedRole = await updatedResponse.Content.ReadFromJsonAsync<IdentityRoleDto>();
                updatedRole.Should().NotBeNull();
                updatedRole!.Name.Should().Be(updatedRoleName);

                var deleteResponse = await Client.DeleteAsync(ById(RolesRoute, createdRoleId));
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
                createdRoleId = null;

                var getDeletedResponse = await Client.GetAsync(ById(RolesRoute, createdRole.Id));
                getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task RoleClaimsCanBeAddedAndDeleted()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            var claimType = UniqueValue(ClaimTypePrefix);
            var claimValue = UniqueValue(ClaimValuePrefix);
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var addClaimRequest = new RoleClaimApiDto<string>
                {
                    ClaimId = 0,
                    RoleId = createdRoleId,
                    ClaimType = claimType,
                    ClaimValue = claimValue
                };

                var addClaimResponse = await Client.PostAsJsonAsync(RoleClaimsRoute, addClaimRequest);
                addClaimResponse.StatusCode.Should().Be(HttpStatusCode.Created);

                var roleClaimsRoute = $"{ById(RolesRoute, createdRoleId)}/{RolesClaimsRouteSuffix}";
                var claimsResponse = await Client.GetAsync($"{roleClaimsRoute}?page={DefaultPage}&pageSize={ClaimsPageSize}");
                claimsResponse.EnsureSuccessStatusCode();

                var claims = await claimsResponse.Content.ReadFromJsonAsync<RoleClaimsApiDto<string>>();
                claims.Should().NotBeNull();
                claims!.Claims.Should().Contain(x => x.ClaimType == claimType && x.ClaimValue == claimValue);

                var createdClaim = claims.Claims.First(x => x.ClaimType == claimType && x.ClaimValue == claimValue);
                var deleteClaimResponse = await Client.DeleteAsync($"{roleClaimsRoute}?claimId={createdClaim.ClaimId}");
                deleteClaimResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                var claimsAfterDeleteResponse = await Client.GetAsync($"{roleClaimsRoute}?page={DefaultPage}&pageSize={ClaimsPageSize}");
                claimsAfterDeleteResponse.EnsureSuccessStatusCode();

                var claimsAfterDelete = await claimsAfterDeleteResponse.Content.ReadFromJsonAsync<RoleClaimsApiDto<string>>();
                claimsAfterDelete.Should().NotBeNull();
                claimsAfterDelete!.Claims.Should().NotContain(x => x.ClaimId == createdClaim.ClaimId);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task RoleClaimsCanBeUpdated()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            var claimType = UniqueValue(ClaimTypePrefix);
            var claimValue = UniqueValue(ClaimValuePrefix);
            var updatedClaimType = UniqueValue(ClaimTypePrefix);
            var updatedClaimValue = UniqueValue(ClaimValuePrefix);
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var createdClaim = await CreateRoleClaimAsync(createdRoleId, claimType, claimValue);

                var updateResponse = await Client.PutAsJsonAsync(RoleClaimsRoute, new RoleClaimApiDto<string>
                {
                    ClaimId = createdClaim.ClaimId,
                    RoleId = createdRoleId,
                    ClaimType = updatedClaimType,
                    ClaimValue = updatedClaimValue
                });
                updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                var roleClaimsRoute = $"{ById(RolesRoute, createdRoleId)}/{RolesClaimsRouteSuffix}";
                var claimsResponse = await Client.GetAsync($"{roleClaimsRoute}?page={DefaultPage}&pageSize={ClaimsPageSize}");
                claimsResponse.EnsureSuccessStatusCode();

                var claims = await claimsResponse.Content.ReadFromJsonAsync<RoleClaimsApiDto<string>>();
                claims.Should().NotBeNull();
                claims!.Claims.Should().Contain(x => x.ClaimType == updatedClaimType && x.ClaimValue == updatedClaimValue);
                claims.Claims.Should().NotContain(x => x.ClaimType == claimType && x.ClaimValue == claimValue);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task RoleClaimsCreateWithExplicitClaimIdReturnsBadRequest()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var response = await Client.PostAsJsonAsync(RoleClaimsRoute, new RoleClaimApiDto<string>
                {
                    ClaimId = NonDefaultClaimId,
                    RoleId = createdRoleId,
                    ClaimType = UniqueValue(ClaimTypePrefix),
                    ClaimValue = UniqueValue(ClaimValuePrefix)
                });

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task RoleClaimsUpdateWithDefaultClaimIdReturnsBadRequest()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            string createdRoleId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var response = await Client.PutAsJsonAsync(RoleClaimsRoute, new RoleClaimApiDto<string>
                {
                    ClaimId = 0,
                    RoleId = createdRoleId,
                    ClaimType = UniqueValue(ClaimTypePrefix),
                    ClaimValue = UniqueValue(ClaimValuePrefix)
                });

                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }

        [Fact]
        public async Task RoleUsersEndpointReturnsAssignedUser()
        {
            SetupAdminAuthorization();

            var roleName = RoleCreateFaker.Generate().Name;
            string createdRoleId = null;
            string createdUserId = null;

            try
            {
                var createdRole = await CreateRoleAsync(roleName);
                createdRoleId = createdRole.Id;

                var createdUser = await CreateUserAsync();
                createdUserId = createdUser.Id;

                var assignRoleResponse = await Client.PostAsJsonAsync(UserRolesRoute, new UserRoleApiDto<string>
                {
                    UserId = createdUserId,
                    RoleId = createdRoleId
                });
                assignRoleResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                var roleUsersRoute = $"{ById(RolesRoute, createdRoleId)}/{RolesUsersRouteSuffix}";
                var roleUsersResponse = await Client.GetAsync(
                    BuildSearchQuery(roleUsersRoute, RoleSearchParameter, createdUser.UserName));
                roleUsersResponse.EnsureSuccessStatusCode();

                var roleUsers = await roleUsersResponse.Content.ReadFromJsonAsync<IdentityUsersDto>();
                roleUsers.Should().NotBeNull();
                roleUsers!.Users.Should().Contain(x => x.Id == createdUserId && x.UserName == createdUser.UserName);

                await DeleteUserRoleAsync(createdUserId, createdRoleId);
            }
            finally
            {
                SetupAdminAuthorization();
                await SafeDeleteAsync(UsersRoute, createdUserId);
                await SafeDeleteAsync(RolesRoute, createdRoleId);
            }
        }
    }
}
