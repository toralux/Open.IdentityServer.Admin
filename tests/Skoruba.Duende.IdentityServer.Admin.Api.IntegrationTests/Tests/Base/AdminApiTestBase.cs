// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Roles;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Users;
using Toralux.Open.IdentityServer.Shared.Dtos.Identity;

namespace Toralux.Open.IdentityServer.Admin.Api.IntegrationTests.Tests.Base
{
    public abstract class AdminApiTestBase : BaseClassFixture
    {
        protected const int DefaultPage = 1;
        protected const int DefaultPageSize = 10;
        protected const int ExtendedPageSize = 20;
        protected const int ClaimsPageSize = 50;
        protected const string UpdatedByIntegrationTest = "Updated by API integration test";

        protected const string RolesRoute = "api/roles";
        protected const string UsersRoute = "api/users";
        protected const string UserRolesRoute = "api/users/roles";
        protected const string RoleClaimsRoute = "api/roles/claims";

        protected static readonly Faker TestDataFaker = new();

        protected static readonly Faker<IdentityRoleDto> RoleCreateFaker = new Faker<IdentityRoleDto>()
            .RuleFor(x => x.Name, f => $"role_integration_{f.Random.AlphaNumeric(12).ToLowerInvariant()}_{Guid.NewGuid():N}");

        protected static readonly Faker<IdentityUserDto> UserCreateFaker = new Faker<IdentityUserDto>()
            .RuleFor(x => x.UserName, f => $"user_integration_{f.Random.AlphaNumeric(12).ToLowerInvariant()}")
            .RuleFor(x => x.Email, (f, u) => $"{u.UserName}@example.com")
            .RuleFor(x => x.EmailConfirmed, false)
            .RuleFor(x => x.PhoneNumber, f => $"+420{f.Random.ReplaceNumbers("#########")}")
            .RuleFor(x => x.PhoneNumberConfirmed, false)
            .RuleFor(x => x.LockoutEnabled, false)
            .RuleFor(x => x.TwoFactorEnabled, false);

        protected AdminApiTestBase(TestFixture fixture) : base(fixture)
        {
        }

        protected void SetupAdminAuthorization()
        {
            Client.DefaultRequestHeaders.Clear();
            SetupAdminClaimsViaHeaders();
        }

        protected void ClearAuthorization()
        {
            Client.DefaultRequestHeaders.Clear();
        }

        protected static string UniqueValue(string prefix)
        {
            var token = TestDataFaker.Random.AlphaNumeric(10).ToLowerInvariant();
            return $"{prefix}_{token}_{Guid.NewGuid():N}";
        }

        protected static string ById(string route, int id) => $"{route}/{id}";

        protected static string ById(string route, string id) => $"{route}/{id}";

        protected static string BuildSearchQuery(string route, string searchParameter, string searchValue, int pageSize = DefaultPageSize)
        {
            var encodedValue = Uri.EscapeDataString(searchValue ?? string.Empty);
            return $"{route}?{searchParameter}={encodedValue}&page={DefaultPage}&pageSize={pageSize}";
        }

        protected static List<string> DistinctStrings(List<string> values)
        {
            return values?
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList() ?? new List<string>();
        }

        protected async Task<IdentityRoleDto> CreateRoleAsync(string roleName)
        {
            var createRequest = RoleCreateFaker.Generate();
            createRequest.Name = roleName;

            var createResponse = await Client.PostAsJsonAsync(RolesRoute, createRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdRole = await createResponse.Content.ReadFromJsonAsync<IdentityRoleDto>();
            createdRole.Should().NotBeNull();
            createdRole!.Id.Should().NotBeNullOrWhiteSpace();
            createdRole.Name.Should().Be(roleName);
            return createdRole;
        }

        protected async Task<IdentityUserDto> CreateUserAsync()
        {
            var createRequest = UserCreateFaker.Generate();

            var createResponse = await Client.PostAsJsonAsync(UsersRoute, createRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdUser = await createResponse.Content.ReadFromJsonAsync<IdentityUserDto>();
            createdUser.Should().NotBeNull();
            createdUser!.Id.Should().NotBeNullOrWhiteSpace();
            createdUser.UserName.Should().Be(createRequest.UserName);
            createdUser.Email.Should().Be(createRequest.Email);
            createdUser.PhoneNumber.Should().Be(createRequest.PhoneNumber);
            return createdUser;
        }

        protected async Task DeleteUserRoleAsync(string userId, string roleId)
        {
            var response = await DeleteBodyAsync(UserRolesRoute, new UserRoleApiDto<string>
            {
                UserId = userId,
                RoleId = roleId
            });

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        protected async Task<RoleClaimApiDto<string>> CreateRoleClaimAsync(string roleId, string claimType, string claimValue)
        {
            var addClaimResponse = await Client.PostAsJsonAsync(RoleClaimsRoute, new RoleClaimApiDto<string>
            {
                ClaimId = 0,
                RoleId = roleId,
                ClaimType = claimType,
                ClaimValue = claimValue
            });
            addClaimResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var roleClaimsRoute = $"{ById(RolesRoute, roleId)}/claims";
            var claimsResponse = await Client.GetAsync($"{roleClaimsRoute}?page={DefaultPage}&pageSize={ClaimsPageSize}");
            claimsResponse.EnsureSuccessStatusCode();

            var claims = await claimsResponse.Content.ReadFromJsonAsync<RoleClaimsApiDto<string>>();
            claims.Should().NotBeNull();
            var createdClaim = claims!.Claims.Find(x => x.ClaimType == claimType && x.ClaimValue == claimValue);
            createdClaim.Should().NotBeNull();

            return createdClaim!;
        }

        protected async Task SafeDeleteAsync(string route, int id)
        {
            if (id > 0)
            {
                await Client.DeleteAsync(ById(route, id));
            }
        }

        protected async Task SafeDeleteAsync(string route, string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await Client.DeleteAsync(ById(route, id));
            }
        }

        protected async Task<HttpResponseMessage> DeleteBodyAsync<TRequest>(string route, TRequest body)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, route)
            {
                Content = JsonContent.Create(body)
            };

            return await Client.SendAsync(request);
        }
    }
}
