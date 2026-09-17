// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Mappers
{
    public class IdentityDataMapperTests
    {
        private static readonly Faker Faker = new();

        [Fact]
        public void MapUserDtoToEntity_DoesNotOverwriteSensitiveIdentityFields()
        {
            var mapper = CreateMapper<SensitiveUserDto, SensitiveUserIdentity>();
            var userId = Faker.Random.Guid().ToString();
            var destination = new SensitiveUserIdentity
            {
                Id = userId,
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email(),
                PasswordHash = Faker.Random.AlphaNumeric(60),
                SecurityStamp = Faker.Random.Guid().ToString(),
                ConcurrencyStamp = Faker.Random.Guid().ToString(),
                NormalizedUserName = Faker.Internet.UserName().ToUpperInvariant(),
                NormalizedEmail = Faker.Internet.Email().ToUpperInvariant()
            };

            var source = new SensitiveUserDto
            {
                Id = userId,
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email(),
                PasswordHash = Faker.Random.AlphaNumeric(60),
                SecurityStamp = Faker.Random.Guid().ToString(),
                ConcurrencyStamp = Faker.Random.Guid().ToString(),
                NormalizedUserName = Faker.Internet.UserName().ToUpperInvariant(),
                NormalizedEmail = Faker.Internet.Email().ToUpperInvariant()
            };

            mapper.MapUserDtoToEntity(source, destination);

            destination.UserName.Should().Be(source.UserName);
            destination.Email.Should().Be(source.Email);
            destination.PasswordHash.Should().NotBe(source.PasswordHash);
            destination.SecurityStamp.Should().NotBe(source.SecurityStamp);
            destination.ConcurrencyStamp.Should().NotBe(source.ConcurrencyStamp);
            destination.NormalizedUserName.Should().NotBe(source.NormalizedUserName);
            destination.NormalizedEmail.Should().NotBe(source.NormalizedEmail);
        }

        [Fact]
        public void MapUserWithHiddenProperties_UsesMostDerivedPropertyWithoutThrowing()
        {
            var mapper = CreateMapper<HiddenPropertyUserDto, HiddenPropertyUserIdentity>();
            var derivedTag = Faker.Random.AlphaNumeric(10);
            var baseTag = Faker.Random.AlphaNumeric(10);
            var sourceEntity = new HiddenPropertyUserIdentity
            {
                Id = Faker.Random.Guid().ToString(),
                UserName = Faker.Internet.UserName(),
                CustomTag = derivedTag
            };

            ((HiddenPropertyUserIdentityBase)sourceEntity).CustomTag = baseTag;

            var dto = mapper.MapUserToDto(sourceEntity);

            dto.CustomTag.Should().Be(derivedTag);
            ((HiddenPropertyUserDtoBase)dto).CustomTag.Should().BeNull();

            var dtoDerivedTag = Faker.Random.AlphaNumeric(10);
            var dtoBaseTag = Faker.Random.AlphaNumeric(10);
            dto.CustomTag = dtoDerivedTag;
            ((HiddenPropertyUserDtoBase)dto).CustomTag = dtoBaseTag;

            var destinationEntity = new HiddenPropertyUserIdentity();
            Action map = () => mapper.MapUserDtoToEntity(dto, destinationEntity);

            map.Should().NotThrow();
            destinationEntity.CustomTag.Should().Be(dtoDerivedTag);
            ((HiddenPropertyUserIdentityBase)destinationEntity).CustomTag.Should().BeNull();
        }

        [Fact]
        public void SanitizeAuditUser_PreservesCustomPropertiesAndRedactsSensitiveFields()
        {
            var mapper = CreateMapper<SensitiveUserDto, SensitiveUserIdentity>(new[] { new SensitiveUserMappingCustomizer() });
            var source = new SensitiveUserDto
            {
                Id = Faker.Random.Guid().ToString(),
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email(),
                PasswordHash = Faker.Random.AlphaNumeric(60),
                SecurityStamp = Faker.Random.Guid().ToString(),
                ConcurrencyStamp = Faker.Random.Guid().ToString(),
                NormalizedUserName = Faker.Internet.UserName().ToUpperInvariant(),
                NormalizedEmail = Faker.Internet.Email().ToUpperInvariant()
            };

            var sanitized = mapper.SanitizeAuditUser(source);

            sanitized.Should().NotBeSameAs(source);
            sanitized.Id.Should().Be(source.Id);
            sanitized.UserName.Should().Be(source.UserName);
            sanitized.Email.Should().Be(source.Email);
            sanitized.NormalizedUserName.Should().Be(source.NormalizedUserName);
            sanitized.NormalizedEmail.Should().Be(source.NormalizedEmail);
            sanitized.PasswordHash.Should().BeNull();
            sanitized.SecurityStamp.Should().BeNull();
            sanitized.ConcurrencyStamp.Should().BeNull();
            source.PasswordHash.Should().NotBeNull();
            source.SecurityStamp.Should().NotBeNull();
            source.ConcurrencyStamp.Should().NotBeNull();
        }

        [Fact]
        public void SanitizeAuditUser_RedactsKnownSensitiveFieldsWithoutCustomAuditSanitizer()
        {
            var mapper = CreateMapper<SensitiveUserDto, SensitiveUserIdentity>();
            var source = new SensitiveUserDto
            {
                Id = Faker.Random.Guid().ToString(),
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email(),
                PasswordHash = Faker.Random.AlphaNumeric(60),
                SecurityStamp = Faker.Random.Guid().ToString(),
                ConcurrencyStamp = Faker.Random.Guid().ToString(),
                NormalizedUserName = Faker.Internet.UserName().ToUpperInvariant(),
                NormalizedEmail = Faker.Internet.Email().ToUpperInvariant()
            };

            var sanitized = mapper.SanitizeAuditUser(source);

            sanitized.Should().NotBeSameAs(source);
            sanitized.PasswordHash.Should().BeNull();
            sanitized.SecurityStamp.Should().BeNull();
            sanitized.ConcurrencyStamp.Should().BeNull();
            sanitized.NormalizedUserName.Should().Be(source.NormalizedUserName);
            sanitized.NormalizedEmail.Should().Be(source.NormalizedEmail);
            source.PasswordHash.Should().NotBeNull();
            source.SecurityStamp.Should().NotBeNull();
            source.ConcurrencyStamp.Should().NotBeNull();
        }

        [Fact]
        public void SanitizeAuditUsers_PreservesMetadataAndDoesNotMutateSourceItems()
        {
            var mapper = CreateMapper<SensitiveUserDto, SensitiveUserIdentity>(new[] { new SensitiveUserMappingCustomizer() });
            var sourceUser = new SensitiveUserDto
            {
                Id = Faker.Random.Guid().ToString(),
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email(),
                PasswordHash = Faker.Random.AlphaNumeric(60),
                SecurityStamp = Faker.Random.Guid().ToString(),
                ConcurrencyStamp = Faker.Random.Guid().ToString(),
                NormalizedUserName = Faker.Internet.UserName().ToUpperInvariant(),
                NormalizedEmail = Faker.Internet.Email().ToUpperInvariant()
            };
            var source = new UsersDto<SensitiveUserDto, string>
            {
                TotalCount = 1,
                PageSize = 10,
                Users = new List<SensitiveUserDto> { sourceUser }
            };

            var sanitized = mapper.SanitizeAuditUsers(source);

            sanitized.Should().NotBeSameAs(source);
            sanitized.TotalCount.Should().Be(source.TotalCount);
            sanitized.PageSize.Should().Be(source.PageSize);
            sanitized.Users.Should().ContainSingle();
            sanitized.Users[0].Should().NotBeSameAs(sourceUser);
            sanitized.Users[0].Id.Should().Be(sourceUser.Id);
            sanitized.Users[0].NormalizedUserName.Should().Be(sourceUser.NormalizedUserName);
            sanitized.Users[0].PasswordHash.Should().BeNull();
            sanitized.Users[0].SecurityStamp.Should().BeNull();
            sanitized.Users[0].ConcurrencyStamp.Should().BeNull();
            sourceUser.PasswordHash.Should().NotBeNull();
            sourceUser.SecurityStamp.Should().NotBeNull();
            sourceUser.ConcurrencyStamp.Should().NotBeNull();
        }

        #region MapUserToDto

        [Fact]
        public void MapUserToDto_MapsAllStandardFields()
        {
            var mapper = CreateDefaultMapper();
            var entity = new IdentityUser
            {
                Id = Faker.Random.Guid().ToString(),
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email(),
                EmailConfirmed = Faker.Random.Bool(),
                PhoneNumber = Faker.Phone.PhoneNumber(),
                PhoneNumberConfirmed = Faker.Random.Bool(),
                LockoutEnabled = Faker.Random.Bool(),
                LockoutEnd = Faker.Date.FutureOffset(),
                TwoFactorEnabled = Faker.Random.Bool(),
                AccessFailedCount = Faker.Random.Int(0, 10)
            };

            var dto = mapper.MapUserToDto(entity);

            dto.Id.Should().Be(entity.Id);
            dto.UserName.Should().Be(entity.UserName);
            dto.Email.Should().Be(entity.Email);
            dto.EmailConfirmed.Should().Be(entity.EmailConfirmed);
            dto.PhoneNumber.Should().Be(entity.PhoneNumber);
            dto.PhoneNumberConfirmed.Should().Be(entity.PhoneNumberConfirmed);
            dto.LockoutEnabled.Should().Be(entity.LockoutEnabled);
            dto.LockoutEnd.Should().Be(entity.LockoutEnd);
            dto.TwoFactorEnabled.Should().Be(entity.TwoFactorEnabled);
            dto.AccessFailedCount.Should().Be(entity.AccessFailedCount);
        }

        #endregion

        #region MapRoleToDto

        [Fact]
        public void MapRoleToDto_MapsIdAndName()
        {
            var mapper = CreateDefaultMapper();
            var entity = new IdentityRole { Id = Faker.Random.Guid().ToString(), Name = Faker.Internet.UserName() };

            var dto = mapper.MapRoleToDto(entity);

            dto.Id.Should().Be(entity.Id);
            dto.Name.Should().Be(entity.Name);
        }

        #endregion

        #region MapUserClaimToClaimsDto

        [Fact]
        public void MapUserClaimToClaimsDto_MapsAllFields()
        {
            var mapper = CreateDefaultMapper();
            var entity = new IdentityUserClaim<string>
            {
                Id = Faker.Random.Int(1, 10000),
                UserId = Faker.Random.Guid().ToString(),
                ClaimType = Faker.Random.Word(),
                ClaimValue = Faker.Random.Word()
            };

            var dto = mapper.MapUserClaimToClaimsDto(entity);

            dto.ClaimId.Should().Be(entity.Id);
            dto.UserId.Should().Be(entity.UserId);
            dto.ClaimType.Should().Be(entity.ClaimType);
            dto.ClaimValue.Should().Be(entity.ClaimValue);
        }

        #endregion

        #region MapRoleClaimToRoleClaimsDto

        [Fact]
        public void MapRoleClaimToRoleClaimsDto_MapsAllFields()
        {
            var mapper = CreateDefaultMapper();
            var entity = new IdentityRoleClaim<string>
            {
                Id = Faker.Random.Int(1, 10000),
                RoleId = Faker.Random.Guid().ToString(),
                ClaimType = Faker.Random.Word(),
                ClaimValue = Faker.Random.Word()
            };

            var dto = mapper.MapRoleClaimToRoleClaimsDto(entity);

            dto.ClaimId.Should().Be(entity.Id);
            dto.RoleId.Should().Be(entity.RoleId);
            dto.ClaimType.Should().Be(entity.ClaimType);
            dto.ClaimValue.Should().Be(entity.ClaimValue);
        }

        [Fact]
        public void MapRoleClaimToRoleClaimsDto_DoesNotSetRoleName()
        {
            // RoleName cannot be derived from IdentityRoleClaim — service must set it explicitly after mapping.
            var mapper = CreateDefaultMapper();
            var entity = new IdentityRoleClaim<string> { Id = Faker.Random.Int(1, 10000), RoleId = Faker.Random.Guid().ToString(), ClaimType = Faker.Random.Word(), ClaimValue = Faker.Random.Word() };

            var dto = mapper.MapRoleClaimToRoleClaimsDto(entity);

            dto.RoleName.Should().BeNull();
        }

        #endregion

        #region MapUserLoginToProviderDto

        [Fact]
        public void MapUserLoginToProviderDto_MapsAllFields()
        {
            var mapper = CreateDefaultMapper();
            var entity = new IdentityUserLogin<string>
            {
                UserId = Faker.Random.Guid().ToString(),
                LoginProvider = Faker.Internet.DomainWord(),
                ProviderKey = Faker.Random.AlphaNumeric(20),
                ProviderDisplayName = Faker.Company.CompanyName()
            };

            var dto = mapper.MapUserLoginToProviderDto(entity);

            dto.UserId.Should().Be(entity.UserId);
            dto.LoginProvider.Should().Be(entity.LoginProvider);
            dto.ProviderKey.Should().Be(entity.ProviderKey);
            dto.ProviderDisplayName.Should().Be(entity.ProviderDisplayName);
        }

        #endregion

        #region MapUserLoginInfosToProvidersDto

        [Fact]
        public void MapUserLoginInfosToProvidersDto_MapsAllLoginInfoFields()
        {
            var mapper = CreateDefaultMapper();
            var first = new UserLoginInfo(Faker.Internet.DomainWord(), Faker.Random.AlphaNumeric(20), Faker.Company.CompanyName());
            var second = new UserLoginInfo(Faker.Internet.DomainWord(), Faker.Random.AlphaNumeric(20), Faker.Company.CompanyName());
            var loginInfos = new List<UserLoginInfo> { first, second };

            var dto = mapper.MapUserLoginInfosToProvidersDto(loginInfos);

            dto.Providers.Should().HaveCount(2);
            dto.Providers[0].LoginProvider.Should().Be(first.LoginProvider);
            dto.Providers[0].ProviderKey.Should().Be(first.ProviderKey);
            dto.Providers[0].ProviderDisplayName.Should().Be(first.ProviderDisplayName);
            dto.Providers[1].LoginProvider.Should().Be(second.LoginProvider);
            dto.Providers[1].ProviderKey.Should().Be(second.ProviderKey);
        }

        #endregion

        #region MapUserDtoToEntity (factory overload)

        [Fact]
        public void MapUserDtoToEntity_WithNonDefaultId_SetsId()
        {
            var mapper = CreateDefaultMapper();
            var userDto = new UserDto<string>
            {
                Id = Faker.Random.Guid().ToString(),
                UserName = Faker.Internet.UserName(),
                Email = Faker.Internet.Email()
            };

            var entity = mapper.MapUserDtoToEntity(userDto);

            entity.Id.Should().Be(userDto.Id);
            entity.UserName.Should().Be(userDto.UserName);
            entity.Email.Should().Be(userDto.Email);
        }

        [Fact]
        public void MapUserDtoToEntity_WithDefaultId_DoesNotOverwriteConstructorId()
        {
            // IdentityUser constructor generates a GUID-based Id.
            // When the DTO Id is default (null), the mapper must not overwrite it with null.
            var mapper = CreateDefaultMapper();
            var userDto = new UserDto<string> { Id = null, UserName = Faker.Internet.UserName() };

            var entity = mapper.MapUserDtoToEntity(userDto);

            entity.Id.Should().NotBeNull("IdentityUser constructor sets a generated Id that must not be overwritten");
            entity.UserName.Should().Be(userDto.UserName);
        }

        #endregion

        #region MapRoleDtoToEntity (factory overload)

        [Fact]
        public void MapRoleDtoToEntity_WithNonDefaultId_SetsId()
        {
            var mapper = CreateDefaultMapper();
            var roleDto = new RoleDto<string> { Id = Faker.Random.Guid().ToString(), Name = Faker.Internet.UserName() };

            var entity = mapper.MapRoleDtoToEntity(roleDto);

            entity.Id.Should().Be(roleDto.Id);
            entity.Name.Should().Be(roleDto.Name);
        }

        #endregion

        #region MapUserClaimsDtoToEntity

        [Fact]
        public void MapUserClaimsDtoToEntity_MapsAllFields()
        {
            var mapper = CreateDefaultMapper();
            var claimsDto = new UserClaimsDto<UserClaimDto<string>, string>
            {
                ClaimId = Faker.Random.Int(1, 10000),
                UserId = Faker.Random.Guid().ToString(),
                ClaimType = Faker.Random.Word(),
                ClaimValue = Faker.Random.Word()
            };

            var entity = mapper.MapUserClaimsDtoToEntity(claimsDto);

            entity.Id.Should().Be(claimsDto.ClaimId);
            entity.UserId.Should().Be(claimsDto.UserId);
            entity.ClaimType.Should().Be(claimsDto.ClaimType);
            entity.ClaimValue.Should().Be(claimsDto.ClaimValue);
        }

        #endregion

        #region MapRoleClaimsDtoToEntity

        [Fact]
        public void MapRoleClaimsDtoToEntity_MapsAllFields()
        {
            var mapper = CreateDefaultMapper();
            var claimsDto = new RoleClaimsDto<RoleClaimDto<string>, string>
            {
                ClaimId = Faker.Random.Int(1, 10000),
                RoleId = Faker.Random.Guid().ToString(),
                ClaimType = Faker.Random.Word(),
                ClaimValue = Faker.Random.Word()
            };

            var entity = mapper.MapRoleClaimsDtoToEntity(claimsDto);

            entity.Id.Should().Be(claimsDto.ClaimId);
            entity.RoleId.Should().Be(claimsDto.RoleId);
            entity.ClaimType.Should().Be(claimsDto.ClaimType);
            entity.ClaimValue.Should().Be(claimsDto.ClaimValue);
        }

        #endregion

        #region Null guards

        [Fact]
        public void MapPagedMethods_WithNullSource_ReturnNull()
        {
            var mapper = CreateDefaultMapper();

            mapper.MapPagedUsersToDto(null).Should().BeNull();
            mapper.MapPagedRolesToRolesDto(null).Should().BeNull();
            mapper.MapPagedRolesToUserRolesDto(null).Should().BeNull();
            mapper.MapPagedUserClaimsToDto(null).Should().BeNull();
            mapper.MapPagedRoleClaimsToDto(null).Should().BeNull();
        }

        [Fact]
        public void MapUserLoginInfosToProvidersDto_WithNullSource_ReturnsEmptyProviders()
        {
            var mapper = CreateDefaultMapper();

            // Intentional contract: return a non-null DTO wrapper with empty Providers for null input.
            var dto = mapper.MapUserLoginInfosToProvidersDto(null);

            dto.Should().NotBeNull();
            dto.Providers.Should().BeEmpty();
        }

        #endregion

        #region MapPagedUsersToDto

        [Fact]
        public void MapPagedUsersToDto_MapsPaginationMetadataAndItems()
        {
            var mapper = CreateDefaultMapper();
            var totalCount = Faker.Random.Int(10, 1000);
            var pageSize = Faker.Random.Int(5, 50);
            var user1 = new IdentityUser { Id = Faker.Random.Guid().ToString(), UserName = Faker.Internet.UserName() };
            var user2 = new IdentityUser { Id = Faker.Random.Guid().ToString(), UserName = Faker.Internet.UserName() };
            var pagedList = new PagedList<IdentityUser> { TotalCount = totalCount, PageSize = pageSize };
            pagedList.Data.Add(user1);
            pagedList.Data.Add(user2);

            var dto = mapper.MapPagedUsersToDto(pagedList);

            dto.TotalCount.Should().Be(totalCount);
            dto.PageSize.Should().Be(pageSize);
            dto.Users.Should().HaveCount(2);
            dto.Users[0].Id.Should().Be(user1.Id);
            dto.Users[1].Id.Should().Be(user2.Id);
        }

        #endregion

        #region MapPagedRolesToRolesDto

        [Fact]
        public void MapPagedRolesToRolesDto_MapsPaginationMetadataAndItems()
        {
            var mapper = CreateDefaultMapper();
            var totalCount = Faker.Random.Int(1, 100);
            var pageSize = Faker.Random.Int(5, 50);
            var role = new IdentityRole { Id = Faker.Random.Guid().ToString(), Name = Faker.Internet.UserName() };
            var pagedList = new PagedList<IdentityRole> { TotalCount = totalCount, PageSize = pageSize };
            pagedList.Data.Add(role);

            var dto = mapper.MapPagedRolesToRolesDto(pagedList);

            dto.TotalCount.Should().Be(totalCount);
            dto.PageSize.Should().Be(pageSize);
            dto.Roles.Should().ContainSingle().Which.Name.Should().Be(role.Name);
        }

        #endregion

        #region MapPagedRolesToUserRolesDto

        [Fact]
        public void MapPagedRolesToUserRolesDto_MapsPaginationMetadataAndItems()
        {
            var mapper = CreateDefaultMapper();
            var totalCount = Faker.Random.Int(1, 100);
            var pageSize = Faker.Random.Int(5, 50);
            var role = new IdentityRole { Id = Faker.Random.Guid().ToString(), Name = Faker.Internet.UserName() };
            var pagedList = new PagedList<IdentityRole> { TotalCount = totalCount, PageSize = pageSize };
            pagedList.Data.Add(role);

            var dto = mapper.MapPagedRolesToUserRolesDto(pagedList);

            dto.TotalCount.Should().Be(totalCount);
            dto.PageSize.Should().Be(pageSize);
            dto.Roles.Should().ContainSingle().Which.Name.Should().Be(role.Name);
        }

        #endregion

        #region MapPagedUserClaimsToDto

        [Fact]
        public void MapPagedUserClaimsToDto_MapsPaginationMetadataAndItems()
        {
            var mapper = CreateDefaultMapper();
            var totalCount = Faker.Random.Int(1, 100);
            var pageSize = Faker.Random.Int(5, 50);
            var claim = new IdentityUserClaim<string> { Id = Faker.Random.Int(1, 10000), UserId = Faker.Random.Guid().ToString(), ClaimType = Faker.Random.Word(), ClaimValue = Faker.Random.Word() };
            var pagedList = new PagedList<IdentityUserClaim<string>> { TotalCount = totalCount, PageSize = pageSize };
            pagedList.Data.Add(claim);

            var dto = mapper.MapPagedUserClaimsToDto(pagedList);

            dto.TotalCount.Should().Be(totalCount);
            dto.PageSize.Should().Be(pageSize);
            dto.Claims.Should().ContainSingle().Which.ClaimType.Should().Be(claim.ClaimType);
        }

        #endregion

        #region MapPagedRoleClaimsToDto

        [Fact]
        public void MapPagedRoleClaimsToDto_MapsPaginationMetadataAndItems()
        {
            var mapper = CreateDefaultMapper();
            var totalCount = Faker.Random.Int(1, 100);
            var pageSize = Faker.Random.Int(5, 50);
            var claim = new IdentityRoleClaim<string> { Id = Faker.Random.Int(1, 10000), RoleId = Faker.Random.Guid().ToString(), ClaimType = Faker.Random.Word(), ClaimValue = Faker.Random.Word() };
            var pagedList = new PagedList<IdentityRoleClaim<string>> { TotalCount = totalCount, PageSize = pageSize };
            pagedList.Data.Add(claim);

            var dto = mapper.MapPagedRoleClaimsToDto(pagedList);

            dto.TotalCount.Should().Be(totalCount);
            dto.PageSize.Should().Be(pageSize);
            dto.Claims.Should().ContainSingle().Which.ClaimType.Should().Be(claim.ClaimType);
        }

        #endregion

        private static IdentityDataMapper<UserDto<string>, RoleDto<string>, IdentityUser, IdentityRole, string,
            IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
            UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>
            CreateDefaultMapper()
        {
            return CreateMapper<UserDto<string>, IdentityUser>();
        }

        private static IdentityDataMapper<TUserDto, RoleDto<string>, TUser, IdentityRole, string,
            IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
            UsersDto<TUserDto, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>
            CreateMapper<TUserDto, TUser>(IEnumerable<IIdentityUserMappingCustomizer<TUserDto, TUser>> userMappingCustomizers = null)
            where TUserDto : UserDto<string>
            where TUser : IdentityUser<string>
        {
            return new IdentityDataMapper<TUserDto, RoleDto<string>, TUser, IdentityRole, string,
                IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>,
                UsersDto<TUserDto, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(
                userMappingCustomizers: userMappingCustomizers);
        }

        public sealed class SensitiveUserDto : UserDto<string>
        {
            public string PasswordHash { get; set; }
            public string SecurityStamp { get; set; }
            public string ConcurrencyStamp { get; set; }
            public string NormalizedUserName { get; set; }
            public string NormalizedEmail { get; set; }
        }

        public sealed class SensitiveUserIdentity : IdentityUser
        {
        }

        public sealed class SensitiveUserMappingCustomizer : IIdentityUserMappingCustomizer<SensitiveUserDto, SensitiveUserIdentity>,
            IIdentityUserAuditSanitizer<SensitiveUserDto>
        {
            public void MapDtoToEntity(SensitiveUserDto source, SensitiveUserIdentity destination)
            {
                destination.PasswordHash = source.PasswordHash;
                destination.SecurityStamp = source.SecurityStamp;
                destination.ConcurrencyStamp = source.ConcurrencyStamp;
            }

            public void MapEntityToDto(SensitiveUserIdentity source, SensitiveUserDto destination)
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

        public class HiddenPropertyUserDtoBase : UserDto<string>
        {
            public string CustomTag { get; set; }
        }

        public sealed class HiddenPropertyUserDto : HiddenPropertyUserDtoBase
        {
            public new string CustomTag { get; set; }
        }

        public class HiddenPropertyUserIdentityBase : IdentityUser
        {
            public string CustomTag { get; set; }
        }

        public sealed class HiddenPropertyUserIdentity : HiddenPropertyUserIdentityBase
        {
            public new string CustomTag { get; set; }
        }
    }
}
