// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Skoruba.AuditLogging.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Events.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Resources;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Toralux.Open.IdentityServer.Shared.Configuration.Constants;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Services
{
    public class IdentityServiceTests
    {
        private static readonly Faker Faker = new();

        public IdentityServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();
            var customDatabaseName = Guid.NewGuid().ToString();

            // Ensure Identity uses SchemaVersion=3 (passkeys) even in in-memory tests
            var services = new ServiceCollection();
            services.AddOptions<IdentityOptions>().Configure(o =>
            {
                o.Stores.SchemaVersion = IdentityStoreDefaults.SchemaVersion;
                o.Stores.MaxLengthForKeys = IdentityStoreDefaults.MaxLengthForKeys;
            });
            var serviceProvider = services.BuildServiceProvider();

            _dbContextOptions = new DbContextOptionsBuilder<AdminIdentityDbContext>()
                .UseApplicationServiceProvider(serviceProvider)
                .UseInMemoryDatabase(databaseName)
                .Options;

            _customDbContextOptions = new DbContextOptionsBuilder<CustomAdminIdentityDbContext>()
                .UseApplicationServiceProvider(serviceProvider)
                .UseInMemoryDatabase(customDatabaseName)
                .Options;
        }

        private readonly DbContextOptions<AdminIdentityDbContext> _dbContextOptions;
        private readonly DbContextOptions<CustomAdminIdentityDbContext> _customDbContextOptions;

        private IIdentityRepository<UserIdentity, UserIdentityRole, string,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken> GetIdentityRepository(AdminIdentityDbContext dbContext,
            UserManager<UserIdentity> userManager,
            RoleManager<UserIdentityRole> roleManager)
        {
            return new IdentityRepository<AdminIdentityDbContext, UserIdentity, UserIdentityRole, string,
                UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
                UserIdentityPasskey>(dbContext, userManager, roleManager);
        }

        private IIdentityService<UserDto<string>, RoleDto<string>, UserIdentity,
            UserIdentityRole, string,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken,
            UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetIdentityService(IIdentityRepository<UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken> identityRepository,
            IIdentityServiceResources identityServiceResources,
            IAuditEventLogger auditEventLogger,
            IIdentityDataMapper<UserDto<string>, RoleDto<string>, UserIdentity, UserIdentityRole, string,
                UserIdentityUserClaim, UserIdentityUserLogin, UserIdentityRoleClaim, UsersDto<UserDto<string>, string>,
                RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>,
                UserProvidersDto<UserProviderDto<string>, string>, RoleClaimsDto<RoleClaimDto<string>, string>,
                UserClaimDto<string>, RoleClaimDto<string>> identityDataMapper)
        {
            return new IdentityService<UserDto<string>, RoleDto<string>, UserIdentity,
                UserIdentityRole, string,
                UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
                UserIdentityUserToken,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(
                identityRepository, identityServiceResources, auditEventLogger, identityDataMapper);
        }

        private static IIdentityDataMapper<UserDto<string>, RoleDto<string>, UserIdentity, UserIdentityRole, string,
            UserIdentityUserClaim, UserIdentityUserLogin, UserIdentityRoleClaim, UsersDto<UserDto<string>, string>,
            RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>,
            UserProvidersDto<UserProviderDto<string>, string>, RoleClaimsDto<RoleClaimDto<string>, string>,
            UserClaimDto<string>, RoleClaimDto<string>> GetIdentityDataMapper()
        {
            return new IdentityDataMapper<UserDto<string>, RoleDto<string>, UserIdentity, UserIdentityRole, string,
                UserIdentityUserClaim, UserIdentityUserLogin, UserIdentityRoleClaim, UsersDto<UserDto<string>, string>,
                RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>,
                UserProvidersDto<UserProviderDto<string>, string>, RoleClaimsDto<RoleClaimDto<string>, string>,
                UserClaimDto<string>, RoleClaimDto<string>>();
        }

        private UserManager<UserIdentity> GetTestUserManager(AdminIdentityDbContext context)
        {
            var testUserManager = IdentityMock.TestUserManager(new UserStore<UserIdentity, UserIdentityRole, AdminIdentityDbContext, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityUserToken, UserIdentityRoleClaim>(context, new IdentityErrorDescriber()));

            return testUserManager;
        }

        private RoleManager<UserIdentityRole> GetTestRoleManager(AdminIdentityDbContext context)
        {
            var testRoleManager = IdentityMock.TestRoleManager(new RoleStore<UserIdentityRole, AdminIdentityDbContext, string, UserIdentityUserRole, UserIdentityRoleClaim>(context, new IdentityErrorDescriber()));

            return testRoleManager;
        }

        private IIdentityService<UserDto<string>, RoleDto<string>, UserIdentity,
            UserIdentityRole, string,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken,
            UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>,
            UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetIdentityService(AdminIdentityDbContext context)
        {
            var testUserManager = GetTestUserManager(context);
            var testRoleManager = GetTestRoleManager(context);

            var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager);
            var localizerIdentityResource = new IdentityServiceResources();
            var identityDataMapper = GetIdentityDataMapper();

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            var auditLogger = auditLoggerMock.Object;

            var identityService = GetIdentityService(identityRepository, localizerIdentityResource, auditLogger, identityDataMapper);

            return identityService;
        }

        private IIdentityService<UserDto<string>, RoleDto<string>, UserIdentity,
            UserIdentityRole, string,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken,
            UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>,
            UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetIdentityService(AdminIdentityDbContext context, IAuditEventLogger auditLogger)
        {
            var testUserManager = GetTestUserManager(context);
            var testRoleManager = GetTestRoleManager(context);

            var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager);
            var localizerIdentityResource = new IdentityServiceResources();
            var identityDataMapper = GetIdentityDataMapper();

            return GetIdentityService(identityRepository, localizerIdentityResource, auditLogger, identityDataMapper);
        }

        private IIdentityService<SensitiveUserDto, RoleDto<string>, UserIdentity,
            UserIdentityRole, string,
            UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
            UserIdentityUserToken,
            UsersDto<SensitiveUserDto, string>, RolesDto<RoleDto<string>, string>,
            UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetSensitiveIdentityService(
            AdminIdentityDbContext context,
            IAuditEventLogger auditLogger)
        {
            var testUserManager = GetTestUserManager(context);
            var testRoleManager = GetTestRoleManager(context);
            var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager);
            var identityDataMapper = new IdentityDataMapper<SensitiveUserDto, RoleDto<string>, UserIdentity, UserIdentityRole, string,
                UserIdentityUserClaim, UserIdentityUserLogin, UserIdentityRoleClaim, UsersDto<SensitiveUserDto, string>,
                RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(
                userMappingCustomizers: new[] { new SensitiveUserMappingCustomizer() });

            return new IdentityService<SensitiveUserDto, RoleDto<string>, UserIdentity,
                UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim,
                UserIdentityUserToken, UsersDto<SensitiveUserDto, string>, RolesDto<RoleDto<string>, string>,
                UserRolesDto<RoleDto<string>, string>, UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>,
                UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(
                identityRepository, new IdentityServiceResources(), auditLogger, identityDataMapper);
        }

        private IIdentityRepository<CustomUserIdentity, CustomRoleIdentity, string,
            CustomUserClaim, CustomUserRole, CustomUserLogin, CustomRoleClaim,
            CustomUserToken> GetCustomIdentityRepository(CustomAdminIdentityDbContext dbContext,
            UserManager<CustomUserIdentity> userManager,
            RoleManager<CustomRoleIdentity> roleManager)
        {
            return new IdentityRepository<CustomAdminIdentityDbContext, CustomUserIdentity, CustomRoleIdentity, string,
                CustomUserClaim, CustomUserRole, CustomUserLogin, CustomRoleClaim, CustomUserToken,
                CustomUserPasskey>(dbContext, userManager, roleManager);
        }

        private IIdentityService<CustomUserDto, CustomRoleDto, CustomUserIdentity,
            CustomRoleIdentity, string,
            CustomUserClaim, CustomUserRole, CustomUserLogin, CustomRoleClaim,
            CustomUserToken,
            UsersDto<CustomUserDto, string>, RolesDto<CustomRoleDto, string>, UserRolesDto<CustomRoleDto, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetCustomIdentityService(
            CustomAdminIdentityDbContext context,
            IEnumerable<IIdentityUserMappingCustomizer<CustomUserDto, CustomUserIdentity>> userMappingCustomizers = null,
            IEnumerable<IIdentityRoleMappingCustomizer<CustomRoleDto, CustomRoleIdentity>> roleMappingCustomizers = null)
        {
            var testUserManager = GetCustomTestUserManager(context);
            var testRoleManager = GetCustomTestRoleManager(context);

            var identityRepository = GetCustomIdentityRepository(context, testUserManager, testRoleManager);
            var localizerIdentityResource = new IdentityServiceResources();

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            var auditLogger = auditLoggerMock.Object;
            var identityDataMapper = new IdentityDataMapper<CustomUserDto, CustomRoleDto, CustomUserIdentity, CustomRoleIdentity, string,
                CustomUserClaim, CustomUserLogin, CustomRoleClaim, UsersDto<CustomUserDto, string>, RolesDto<CustomRoleDto, string>,
                UserRolesDto<CustomRoleDto, string>, UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>,
                UserProvidersDto<UserProviderDto<string>, string>, RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>,
                RoleClaimDto<string>>(userMappingCustomizers, roleMappingCustomizers);

            return new IdentityService<CustomUserDto, CustomRoleDto, CustomUserIdentity,
                CustomRoleIdentity, string,
                CustomUserClaim, CustomUserRole, CustomUserLogin, CustomRoleClaim,
                CustomUserToken,
                UsersDto<CustomUserDto, string>, RolesDto<CustomRoleDto, string>, UserRolesDto<CustomRoleDto, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(
                identityRepository,
                localizerIdentityResource,
                auditLogger,
                identityDataMapper);
        }

        private UserManager<CustomUserIdentity> GetCustomTestUserManager(CustomAdminIdentityDbContext context)
        {
            return IdentityMock.TestUserManager(new UserStore<CustomUserIdentity, CustomRoleIdentity, CustomAdminIdentityDbContext, string,
                CustomUserClaim, CustomUserRole, CustomUserLogin, CustomUserToken, CustomRoleClaim>(context, new IdentityErrorDescriber()));
        }

        private RoleManager<CustomRoleIdentity> GetCustomTestRoleManager(CustomAdminIdentityDbContext context)
        {
            return IdentityMock.TestRoleManager(new RoleStore<CustomRoleIdentity, CustomAdminIdentityDbContext, string, CustomUserRole, CustomRoleClaim>(context, new IdentityErrorDescriber()));
        }

        private static CustomUserDto CreateCustomUserDto(string id = null, string displayName = null, string nickName = null)
        {
            return new CustomUserDto
            {
                Id = id,
                UserName = Guid.NewGuid().ToString(),
                Email = $"{Guid.NewGuid():N}@test.local",
                EmailConfirmed = true,
                PhoneNumber = "123456789",
                PhoneNumberConfirmed = true,
                LockoutEnabled = true,
                LockoutEnd = DateTimeOffset.UtcNow.AddDays(1),
                TwoFactorEnabled = true,
                AccessFailedCount = 3,
                DisplayName = displayName ?? Guid.NewGuid().ToString(),
                NickName = nickName
            };
        }

        private static CustomRoleDto CreateCustomRoleDto(string id = null, string description = null, string externalLabel = null)
        {
            return new CustomRoleDto
            {
                Id = id,
                Name = Guid.NewGuid().ToString(),
                Description = description ?? Guid.NewGuid().ToString(),
                ExternalLabel = externalLabel
            };
        }

        [Fact]
        public async Task AddUserAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);
            }
        }

        [Fact]
        public async Task GetUserAsync_AuditEvent_DoesNotContainPasswordHash()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var userId = Guid.NewGuid().ToString();
                var seedUserDto = IdentityDtoMock<string>.GenerateRandomUser(userId);
                var passwordHash = Faker.Random.AlphaNumeric(60);

                await context.Users.AddAsync(new UserIdentity
                {
                    Id = seedUserDto.Id,
                    UserName = seedUserDto.UserName,
                    NormalizedUserName = seedUserDto.UserName.ToUpperInvariant(),
                    Email = seedUserDto.Email,
                    NormalizedEmail = seedUserDto.Email.ToUpperInvariant(),
                    EmailConfirmed = seedUserDto.EmailConfirmed,
                    PhoneNumber = seedUserDto.PhoneNumber,
                    PhoneNumberConfirmed = seedUserDto.PhoneNumberConfirmed,
                    LockoutEnabled = seedUserDto.LockoutEnabled,
                    LockoutEnd = seedUserDto.LockoutEnd,
                    TwoFactorEnabled = seedUserDto.TwoFactorEnabled,
                    AccessFailedCount = seedUserDto.AccessFailedCount,
                    PasswordHash = passwordHash
                });
                await context.SaveChangesAsync();

                var auditLoggerMock = new Mock<IAuditEventLogger>();
                auditLoggerMock
                    .Setup(x => x.LogEventAsync(It.IsAny<UserRequestedEvent<UserDto<string>>>()))
                    .Returns(Task.CompletedTask);

                var identityService = GetIdentityService(context, auditLoggerMock.Object);

                var loadedUserDto = await identityService.GetUserAsync(userId);

                loadedUserDto.Should().NotBeNull();
                auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<UserRequestedEvent<UserDto<string>>>(e =>
                    !JsonSerializer.Serialize(e).Contains("PasswordHash") &&
                    !JsonSerializer.Serialize(e).Contains(passwordHash))), Times.Once);
            }
        }

        [Fact]
        public async Task GetUserAsync_AuditEvent_SanitizesSensitivePropertiesForCustomUserDto()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var userId = Guid.NewGuid().ToString();
                var seedUserDto = IdentityDtoMock<string>.GenerateRandomUser(userId);
                var passwordHash = Faker.Random.AlphaNumeric(60);
                var securityStamp = Faker.Random.Guid().ToString();
                var concurrencyStamp = Faker.Random.Guid().ToString();

                await context.Users.AddAsync(new UserIdentity
                {
                    Id = seedUserDto.Id,
                    UserName = seedUserDto.UserName,
                    NormalizedUserName = seedUserDto.UserName.ToUpperInvariant(),
                    Email = seedUserDto.Email,
                    NormalizedEmail = seedUserDto.Email.ToUpperInvariant(),
                    EmailConfirmed = seedUserDto.EmailConfirmed,
                    PhoneNumber = seedUserDto.PhoneNumber,
                    PhoneNumberConfirmed = seedUserDto.PhoneNumberConfirmed,
                    LockoutEnabled = seedUserDto.LockoutEnabled,
                    LockoutEnd = seedUserDto.LockoutEnd,
                    TwoFactorEnabled = seedUserDto.TwoFactorEnabled,
                    AccessFailedCount = seedUserDto.AccessFailedCount,
                    PasswordHash = passwordHash,
                    SecurityStamp = securityStamp,
                    ConcurrencyStamp = concurrencyStamp
                });
                await context.SaveChangesAsync();

                var auditLoggerMock = new Mock<IAuditEventLogger>();
                auditLoggerMock
                    .Setup(x => x.LogEventAsync(It.IsAny<UserRequestedEvent<SensitiveUserDto>>()))
                    .Returns(Task.CompletedTask);

                var identityService = GetSensitiveIdentityService(context, auditLoggerMock.Object);

                var loadedUserDto = await identityService.GetUserAsync(userId);

                loadedUserDto.PasswordHash.Should().Be(passwordHash);
                loadedUserDto.SecurityStamp.Should().Be(securityStamp);
                loadedUserDto.ConcurrencyStamp.Should().Be(concurrencyStamp);
                auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<UserRequestedEvent<SensitiveUserDto>>(e =>
                    e.UserDto.PasswordHash == null &&
                    e.UserDto.SecurityStamp == null &&
                    e.UserDto.ConcurrencyStamp == null)), Times.Once);
            }
        }

        [Fact]
        public async Task GetUsersAsync_AuditEvent_SanitizesSensitivePropertiesForCustomUserDto()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var userId = Guid.NewGuid().ToString();
                var seedUserDto = IdentityDtoMock<string>.GenerateRandomUser(userId);
                var passwordHash = Faker.Random.AlphaNumeric(60);
                var securityStamp = Faker.Random.Guid().ToString();
                var concurrencyStamp = Faker.Random.Guid().ToString();

                await context.Users.AddAsync(new UserIdentity
                {
                    Id = seedUserDto.Id,
                    UserName = seedUserDto.UserName,
                    NormalizedUserName = seedUserDto.UserName.ToUpperInvariant(),
                    Email = seedUserDto.Email,
                    NormalizedEmail = seedUserDto.Email.ToUpperInvariant(),
                    EmailConfirmed = seedUserDto.EmailConfirmed,
                    PhoneNumber = seedUserDto.PhoneNumber,
                    PhoneNumberConfirmed = seedUserDto.PhoneNumberConfirmed,
                    LockoutEnabled = seedUserDto.LockoutEnabled,
                    LockoutEnd = seedUserDto.LockoutEnd,
                    TwoFactorEnabled = seedUserDto.TwoFactorEnabled,
                    AccessFailedCount = seedUserDto.AccessFailedCount,
                    PasswordHash = passwordHash,
                    SecurityStamp = securityStamp,
                    ConcurrencyStamp = concurrencyStamp
                });
                await context.SaveChangesAsync();

                var auditLoggerMock = new Mock<IAuditEventLogger>();
                auditLoggerMock
                    .Setup(x => x.LogEventAsync(It.IsAny<UsersRequestedEvent<UsersDto<SensitiveUserDto, string>>>()))
                    .Returns(Task.CompletedTask);

                var identityService = GetSensitiveIdentityService(context, auditLoggerMock.Object);

                var loadedUsersDto = await identityService.GetUsersAsync(string.Empty);

                loadedUsersDto.Users.Should().ContainSingle();
                loadedUsersDto.Users[0].PasswordHash.Should().Be(passwordHash);
                loadedUsersDto.Users[0].SecurityStamp.Should().Be(securityStamp);
                loadedUsersDto.Users[0].ConcurrencyStamp.Should().Be(concurrencyStamp);
                auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<UsersRequestedEvent<UsersDto<SensitiveUserDto, string>>>(e =>
                    e.Users.Users.Count == 1 &&
                    e.Users.Users.All(u =>
                        u.PasswordHash == null &&
                        u.SecurityStamp == null &&
                        u.ConcurrencyStamp == null))), Times.Once);
            }
        }

        [Fact]
        public async Task GetRoleUsersAsync_AuditEvent_SanitizesSensitivePropertiesForCustomUserDto()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var userId = Guid.NewGuid().ToString();
                var roleId = Guid.NewGuid().ToString();
                var seedUserDto = IdentityDtoMock<string>.GenerateRandomUser(userId);
                var passwordHash = Faker.Random.AlphaNumeric(60);
                var securityStamp = Faker.Random.Guid().ToString();
                var concurrencyStamp = Faker.Random.Guid().ToString();

                await context.Users.AddAsync(new UserIdentity
                {
                    Id = seedUserDto.Id,
                    UserName = seedUserDto.UserName,
                    NormalizedUserName = seedUserDto.UserName.ToUpperInvariant(),
                    Email = seedUserDto.Email,
                    NormalizedEmail = seedUserDto.Email.ToUpperInvariant(),
                    EmailConfirmed = seedUserDto.EmailConfirmed,
                    PhoneNumber = seedUserDto.PhoneNumber,
                    PhoneNumberConfirmed = seedUserDto.PhoneNumberConfirmed,
                    LockoutEnabled = seedUserDto.LockoutEnabled,
                    LockoutEnd = seedUserDto.LockoutEnd,
                    TwoFactorEnabled = seedUserDto.TwoFactorEnabled,
                    AccessFailedCount = seedUserDto.AccessFailedCount,
                    PasswordHash = passwordHash,
                    SecurityStamp = securityStamp,
                    ConcurrencyStamp = concurrencyStamp
                });
                await context.Roles.AddAsync(new UserIdentityRole
                {
                    Id = roleId,
                    Name = "test-role",
                    NormalizedName = "TEST-ROLE"
                });
                await context.UserRoles.AddAsync(new UserIdentityUserRole
                {
                    UserId = userId,
                    RoleId = roleId
                });
                await context.SaveChangesAsync();

                var auditLoggerMock = new Mock<IAuditEventLogger>();
                auditLoggerMock
                    .Setup(x => x.LogEventAsync(It.IsAny<RoleUsersRequestedEvent<UsersDto<SensitiveUserDto, string>>>()))
                    .Returns(Task.CompletedTask);

                var identityService = GetSensitiveIdentityService(context, auditLoggerMock.Object);

                var loadedUsersDto = await identityService.GetRoleUsersAsync(roleId, string.Empty);

                loadedUsersDto.Users.Should().ContainSingle();
                loadedUsersDto.Users[0].PasswordHash.Should().Be(passwordHash);
                loadedUsersDto.Users[0].SecurityStamp.Should().Be(securityStamp);
                loadedUsersDto.Users[0].ConcurrencyStamp.Should().Be(concurrencyStamp);
                auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<RoleUsersRequestedEvent<UsersDto<SensitiveUserDto, string>>>(e =>
                    e.Users.Users.Count == 1 &&
                    e.Users.Users.All(u =>
                        u.PasswordHash == null &&
                        u.SecurityStamp == null &&
                        u.ConcurrencyStamp == null))), Times.Once);
            }
        }

        [Fact]
        public async Task GetClaimUsersAsync_AuditEvent_SanitizesSensitivePropertiesForCustomUserDto()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var userId = Guid.NewGuid().ToString();
                var seedUserDto = IdentityDtoMock<string>.GenerateRandomUser(userId);
                var passwordHash = Faker.Random.AlphaNumeric(60);
                var securityStamp = Faker.Random.Guid().ToString();
                var concurrencyStamp = Faker.Random.Guid().ToString();
                var claimType = "department";
                var claimValue = "engineering";

                await context.Users.AddAsync(new UserIdentity
                {
                    Id = seedUserDto.Id,
                    UserName = seedUserDto.UserName,
                    NormalizedUserName = seedUserDto.UserName.ToUpperInvariant(),
                    Email = seedUserDto.Email,
                    NormalizedEmail = seedUserDto.Email.ToUpperInvariant(),
                    EmailConfirmed = seedUserDto.EmailConfirmed,
                    PhoneNumber = seedUserDto.PhoneNumber,
                    PhoneNumberConfirmed = seedUserDto.PhoneNumberConfirmed,
                    LockoutEnabled = seedUserDto.LockoutEnabled,
                    LockoutEnd = seedUserDto.LockoutEnd,
                    TwoFactorEnabled = seedUserDto.TwoFactorEnabled,
                    AccessFailedCount = seedUserDto.AccessFailedCount,
                    PasswordHash = passwordHash,
                    SecurityStamp = securityStamp,
                    ConcurrencyStamp = concurrencyStamp
                });
                await context.UserClaims.AddAsync(new UserIdentityUserClaim
                {
                    UserId = userId,
                    ClaimType = claimType,
                    ClaimValue = claimValue
                });
                await context.SaveChangesAsync();

                var auditLoggerMock = new Mock<IAuditEventLogger>();
                auditLoggerMock
                    .Setup(x => x.LogEventAsync(It.IsAny<ClaimUsersRequestedEvent<UsersDto<SensitiveUserDto, string>>>()))
                    .Returns(Task.CompletedTask);

                var identityService = GetSensitiveIdentityService(context, auditLoggerMock.Object);

                var loadedUsersDto = await identityService.GetClaimUsersAsync(claimType, claimValue);

                loadedUsersDto.Users.Should().ContainSingle();
                loadedUsersDto.Users[0].PasswordHash.Should().Be(passwordHash);
                loadedUsersDto.Users[0].SecurityStamp.Should().Be(securityStamp);
                loadedUsersDto.Users[0].ConcurrencyStamp.Should().Be(concurrencyStamp);
                auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<ClaimUsersRequestedEvent<UsersDto<SensitiveUserDto, string>>>(e =>
                    e.Users.Users.Count == 1 &&
                    e.Users.Users.All(u =>
                        u.PasswordHash == null &&
                        u.SecurityStamp == null &&
                        u.ConcurrencyStamp == null))), Times.Once);
            }
        }

        [Fact]
        public async Task DeleteUserProviderAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                var userProvider = IdentityMock.GenerateRandomUserProviders(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    newUserDto.Id);

                //Add new user login
                await context.UserLogins.AddAsync(userProvider);
                await context.SaveChangesAsync();

                //Get added user provider
                var addedUserProvider = await context.UserLogins.Where(x => x.ProviderKey == userProvider.ProviderKey && x.LoginProvider == userProvider.LoginProvider).SingleOrDefaultAsync();
                addedUserProvider.Should().NotBeNull();

                var userProviderDto = IdentityDtoMock<string>.GenerateRandomUserProviders(userProvider.ProviderKey, userProvider.LoginProvider,
                    userProvider.UserId);

                await identityService.DeleteUserProvidersAsync(userProviderDto);

                //Get deleted user provider
                var deletedUserProvider = await context.UserLogins.Where(x => x.ProviderKey == userProvider.ProviderKey && x.LoginProvider == userProvider.LoginProvider).SingleOrDefaultAsync();
                deletedUserProvider.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddUserRoleAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                var userRoleDto = IdentityDtoMock<string>.GenerateRandomUserRole<RoleDto<string>>(roleDto.Id, userDto.Id);

                await identityService.CreateUserRoleAsync(userRoleDto);

                //Get new role
                var userRole = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();

                userRole.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task DeleteUserRoleAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                var userRoleDto = IdentityDtoMock<string>.GenerateRandomUserRole<RoleDto<string>>(roleDto.Id, userDto.Id);

                await identityService.CreateUserRoleAsync(userRoleDto);

                //Get new role
                var userRole = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();
                userRole.Should().NotBeNull();

                await identityService.DeleteUserRoleAsync(userRoleDto);

                //Get deleted role
                var userRoleDeleted = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();
                userRoleDeleted.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddUserClaimAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new user claim
                var userClaimDto = IdentityDtoMock<string>.GenerateRandomUserClaim(0, userDto.Id);

                await identityService.CreateUserClaimsAsync(userClaimDto);

                //Get new user claim
                var claim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                userClaimDto.ClaimId = claim.Id;

                var newUserClaim = await identityService.GetUserClaimAsync(userDto.Id.ToString(), claim.Id);

                //Assert new user claim
                newUserClaim.Should().BeEquivalentTo(userClaimDto);
            }
        }

        [Fact]
        public async Task DeleteUserClaimAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new user claim
                var userClaimDto = IdentityDtoMock<string>.GenerateRandomUserClaim(0, userDto.Id);

                await identityService.CreateUserClaimsAsync(userClaimDto);

                //Get new user claim
                var claim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                userClaimDto.ClaimId = claim.Id;

                var newUserClaim = await identityService.GetUserClaimAsync(userDto.Id.ToString(), claim.Id);

                //Assert new user claim
                newUserClaim.Should().BeEquivalentTo(userClaimDto);

                await identityService.DeleteUserClaimAsync(userClaimDto);

                //Get deleted user claim
                var deletedClaim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                deletedClaim.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateUserAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Detached the added item
                context.Entry(user).State = EntityState.Detached;

                //Generete new user with added item id
                var userDtoForUpdate = IdentityDtoMock<string>.GenerateRandomUser(user.Id);

                //Update user
                await identityService.UpdateUserAsync(userDtoForUpdate);

                var updatedUser = await identityService.GetUserAsync(userDtoForUpdate.Id.ToString());

                //Assert updated user
                updatedUser.Should().BeEquivalentTo(userDtoForUpdate);
            }
        }

        [Fact]
        public async Task DeleteUserAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Remove user
                await identityService.DeleteUserAsync(newUserDto.Id.ToString(), newUserDto);

                //Try Get Removed user
                var removeUser = await context.Users.Where(x => x.Id == user.Id)
                    .SingleOrDefaultAsync();

                //Assert removed user
                removeUser.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRoleAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);
            }
        }

        [Fact]
        public async Task UpdateRoleAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Detached the added item
                context.Entry(role).State = EntityState.Detached;

                //Generete new role with added item id
                var roleDtoForUpdate = IdentityDtoMock<string>.GenerateRandomRole(role.Id);

                //Update role
                await identityService.UpdateRoleAsync(roleDtoForUpdate);

                var updatedRole = await identityService.GetRoleAsync(roleDtoForUpdate.Id.ToString());

                //Assert updated role
                updatedRole.Should().BeEquivalentTo(roleDtoForUpdate);
            }
        }

        [Fact]
        public async Task DeleteRoleAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Remove role
                await identityService.DeleteRoleAsync(newRoleDto);

                //Try Get Removed role
                var removeRole = await context.Roles.Where(x => x.Id == role.Id)
                    .SingleOrDefaultAsync();

                //Assert removed role
                removeRole.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRoleClaimAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<string>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get new role claim
                var roleClaim = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();
                roleClaimDto.ClaimId = roleClaim.Id;

                var newRoleClaimDto = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaimDto.ClaimId);

                //Assert new role
                newRoleClaimDto.Should().BeEquivalentTo(roleClaimDto, options => options.Excluding(o => o.RoleName));
            }
        }

        [Fact]
        public async Task RemoveRoleClaimAsync()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<string>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get new role claim
                var roleClaim = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();
                roleClaimDto.ClaimId = roleClaim.Id;

                var newRoleClaimDto = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaimDto.ClaimId);

                //Assert new role
                newRoleClaimDto.Should().BeEquivalentTo(roleClaimDto, options => options.Excluding(o => o.RoleName));

                await identityService.DeleteRoleClaimAsync(roleClaimDto);

                var roleClaimToDelete = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();

                //Assert removed role claim
                roleClaimToDelete.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetRoleClaimAsync_ReturnsRoleNamePopulated()
        {
            using (var context = new AdminIdentityDbContext(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role with its persisted Id
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<string>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get persisted claim Id
                var roleClaim = await context.RoleClaims
                    .Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue)
                    .SingleOrDefaultAsync();

                //Act
                var result = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaim.Id);

                //Assert — RoleName must be set by the service (mapper cannot derive it from IdentityRoleClaim)
                result.RoleName.Should().Be(role.Name);
                result.ClaimId.Should().Be(roleClaim.Id);
                result.ClaimType.Should().Be(roleClaimDto.ClaimType);
                result.ClaimValue.Should().Be(roleClaimDto.ClaimValue);
            }
        }

        [Fact]
        public async Task CustomDerivedUserPropertiesAreMappedEndToEnd()
        {
            using (var context = new CustomAdminIdentityDbContext(_customDbContextOptions))
            {
                var identityService = GetCustomIdentityService(context);

                var userDto = CreateCustomUserDto(displayName: "initial-display-name");

                await identityService.CreateUserAsync(userDto);

                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                user.DisplayName.Should().Be(userDto.DisplayName);

                var newUserDto = await identityService.GetUserAsync(userDto.Id);
                newUserDto.Should().BeEquivalentTo(userDto);

                context.Entry(user).State = EntityState.Detached;

                var userDtoForUpdate = CreateCustomUserDto(user.Id, "updated-display-name");

                await identityService.UpdateUserAsync(userDtoForUpdate);

                var updatedUser = await identityService.GetUserAsync(user.Id);
                updatedUser.Should().BeEquivalentTo(userDtoForUpdate);

                var persistedUser = await context.Users.Where(x => x.Id == user.Id).SingleOrDefaultAsync();
                persistedUser.DisplayName.Should().Be(userDtoForUpdate.DisplayName);
            }
        }

        [Fact]
        public async Task CustomDerivedRolePropertiesAreMappedEndToEnd()
        {
            using (var context = new CustomAdminIdentityDbContext(_customDbContextOptions))
            {
                var identityService = GetCustomIdentityService(context);

                var roleDto = CreateCustomRoleDto(description: "initial-description");

                await identityService.CreateRoleAsync(roleDto);

                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                role.Description.Should().Be(roleDto.Description);

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id);
                newRoleDto.Should().BeEquivalentTo(roleDto);

                context.Entry(role).State = EntityState.Detached;

                var roleDtoForUpdate = CreateCustomRoleDto(role.Id, "updated-description");

                await identityService.UpdateRoleAsync(roleDtoForUpdate);

                var updatedRole = await identityService.GetRoleAsync(role.Id);
                updatedRole.Should().BeEquivalentTo(roleDtoForUpdate);

                var persistedRole = await context.Roles.Where(x => x.Id == role.Id).SingleOrDefaultAsync();
                persistedRole.Description.Should().Be(roleDtoForUpdate.Description);
            }
        }

        [Fact]
        public async Task CustomUserMappingCustomizerCanMapMismatchedPropertyNames()
        {
            using (var context = new CustomAdminIdentityDbContext(_customDbContextOptions))
            {
                var identityService = GetCustomIdentityService(context,
                    userMappingCustomizers: new[] { new CustomUserMappingCustomizer() });

                var userDto = CreateCustomUserDto(displayName: "display", nickName: "nick-1");
                await identityService.CreateUserAsync(userDto);

                var persistedUser = await context.Users.AsNoTracking().Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                persistedUser.PreferredName.Should().Be("nick-1");

                var loadedUserDto = await identityService.GetUserAsync(persistedUser.Id);
                loadedUserDto.NickName.Should().Be("nick-1");

                loadedUserDto.NickName = "nick-2";
                await identityService.UpdateUserAsync(loadedUserDto);

                context.ChangeTracker.Clear();

                var persistedUpdatedUser = await context.Users.AsNoTracking().Where(x => x.Id == persistedUser.Id).SingleOrDefaultAsync();
                persistedUpdatedUser.PreferredName.Should().Be("nick-2");

                var loadedUpdatedUserDto = await identityService.GetUserAsync(persistedUser.Id);
                loadedUpdatedUserDto.NickName.Should().Be("nick-2");
            }
        }

        [Fact]
        public async Task CustomRoleMappingCustomizerCanMapMismatchedPropertyNames()
        {
            using (var context = new CustomAdminIdentityDbContext(_customDbContextOptions))
            {
                var identityService = GetCustomIdentityService(context,
                    roleMappingCustomizers: new[] { new CustomRoleMappingCustomizer() });

                var roleDto = CreateCustomRoleDto(description: "desc", externalLabel: "label-1");
                await identityService.CreateRoleAsync(roleDto);

                var persistedRole = await context.Roles.AsNoTracking().Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                persistedRole.InternalLabel.Should().Be("label-1");

                var loadedRoleDto = await identityService.GetRoleAsync(persistedRole.Id);
                loadedRoleDto.ExternalLabel.Should().Be("label-1");

                loadedRoleDto.ExternalLabel = "label-2";
                await identityService.UpdateRoleAsync(loadedRoleDto);

                context.ChangeTracker.Clear();

                var persistedUpdatedRole = await context.Roles.AsNoTracking().Where(x => x.Id == persistedRole.Id).SingleOrDefaultAsync();
                persistedUpdatedRole.InternalLabel.Should().Be("label-2");

                var loadedUpdatedRoleDto = await identityService.GetRoleAsync(persistedRole.Id);
                loadedUpdatedRoleDto.ExternalLabel.Should().Be("label-2");
            }
        }

        public sealed class CustomUserDto : UserDto<string>
        {
            public string DisplayName { get; set; }
            public string NickName { get; set; }
        }

        public sealed class SensitiveUserDto : UserDto<string>
        {
            public string PasswordHash { get; set; }
            public string SecurityStamp { get; set; }
            public string ConcurrencyStamp { get; set; }
        }

        public sealed class CustomRoleDto : RoleDto<string>
        {
            public string Description { get; set; }
            public string ExternalLabel { get; set; }
        }

        public sealed class CustomUserIdentity : IdentityUser
        {
            public string DisplayName { get; set; }
            public string PreferredName { get; set; }
        }

        public sealed class CustomRoleIdentity : IdentityRole
        {
            public string Description { get; set; }
            public string InternalLabel { get; set; }
        }

        public sealed class CustomUserMappingCustomizer : IIdentityUserMappingCustomizer<CustomUserDto, CustomUserIdentity>
        {
            public void MapDtoToEntity(CustomUserDto source, CustomUserIdentity destination)
            {
                destination.PreferredName = source.NickName;
            }

            public void MapEntityToDto(CustomUserIdentity source, CustomUserDto destination)
            {
                destination.NickName = source.PreferredName;
            }
        }

        public sealed class SensitiveUserMappingCustomizer : IIdentityUserMappingCustomizer<SensitiveUserDto, UserIdentity>,
            IIdentityUserAuditSanitizer<SensitiveUserDto>
        {
            public void MapDtoToEntity(SensitiveUserDto source, UserIdentity destination)
            {
                destination.PasswordHash = source.PasswordHash;
                destination.SecurityStamp = source.SecurityStamp;
                destination.ConcurrencyStamp = source.ConcurrencyStamp;
            }

            public void MapEntityToDto(UserIdentity source, SensitiveUserDto destination)
            {
                destination.PasswordHash = source.PasswordHash;
                destination.SecurityStamp = source.SecurityStamp;
                destination.ConcurrencyStamp = source.ConcurrencyStamp;
            }

            public void SanitizeAuditUser(SensitiveUserDto user)
            {
                user.PasswordHash = null;
                user.SecurityStamp = null;
                user.ConcurrencyStamp = null;
            }
        }

        public sealed class CustomRoleMappingCustomizer : IIdentityRoleMappingCustomizer<CustomRoleDto, CustomRoleIdentity>
        {
            public void MapDtoToEntity(CustomRoleDto source, CustomRoleIdentity destination)
            {
                destination.InternalLabel = source.ExternalLabel;
            }

            public void MapEntityToDto(CustomRoleIdentity source, CustomRoleDto destination)
            {
                destination.ExternalLabel = source.InternalLabel;
            }
        }

        public sealed class CustomUserClaim : IdentityUserClaim<string>
        {
        }

        public sealed class CustomUserRole : IdentityUserRole<string>
        {
        }

        public sealed class CustomUserLogin : IdentityUserLogin<string>
        {
        }

        public sealed class CustomRoleClaim : IdentityRoleClaim<string>
        {
        }

        public sealed class CustomUserToken : IdentityUserToken<string>
        {
        }

        public sealed class CustomUserPasskey : IdentityUserPasskey<string>
        {
        }

        public sealed class CustomAdminIdentityDbContext : IdentityDbContext<CustomUserIdentity, CustomRoleIdentity, string,
            CustomUserClaim, CustomUserRole, CustomUserLogin, CustomRoleClaim, CustomUserToken, CustomUserPasskey>
        {
            public CustomAdminIdentityDbContext(DbContextOptions<CustomAdminIdentityDbContext> options)
                : base(options)
            {
            }
        }
    }
}
