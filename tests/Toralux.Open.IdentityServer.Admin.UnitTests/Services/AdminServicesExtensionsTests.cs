// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Extensions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Services
{
    public class AdminServicesExtensionsTests
    {
        [Fact]
        public void AddIdentityUserMappingCustomizer_RegistersCustomizer()
        {
            var services = new ServiceCollection();

            services.AddIdentityUserMappingCustomizer<TestUserDto, TestUser, TestUserMappingCustomizer>();

            using var provider = services.BuildServiceProvider();
            var customizers = provider.GetServices<IIdentityUserMappingCustomizer<TestUserDto, TestUser>>();

            customizers.Should().ContainSingle().Which.Should().BeOfType<TestUserMappingCustomizer>();
        }

        [Fact]
        public void AddIdentityRoleMappingCustomizer_RegistersCustomizer()
        {
            var services = new ServiceCollection();

            services.AddIdentityRoleMappingCustomizer<TestRoleDto, TestRole, TestRoleMappingCustomizer>();

            using var provider = services.BuildServiceProvider();
            var customizers = provider.GetServices<IIdentityRoleMappingCustomizer<TestRoleDto, TestRole>>();

            customizers.Should().ContainSingle().Which.Should().BeOfType<TestRoleMappingCustomizer>();
        }

        public sealed class TestUserDto : UserDto<string>
        {
        }

        public sealed class TestUser : IdentityUser
        {
        }

        public sealed class TestRoleDto : RoleDto<string>
        {
        }

        public sealed class TestRole : IdentityRole
        {
        }

        public sealed class TestUserMappingCustomizer : IIdentityUserMappingCustomizer<TestUserDto, TestUser>
        {
            public void MapDtoToEntity(TestUserDto source, TestUser destination)
            {
            }

            public void MapEntityToDto(TestUser source, TestUserDto destination)
            {
            }
        }

        public sealed class TestRoleMappingCustomizer : IIdentityRoleMappingCustomizer<TestRoleDto, TestRole>
        {
            public void MapDtoToEntity(TestRoleDto source, TestRole destination)
            {
            }

            public void MapEntityToDto(TestRole source, TestRoleDto destination)
            {
            }
        }
    }
}
