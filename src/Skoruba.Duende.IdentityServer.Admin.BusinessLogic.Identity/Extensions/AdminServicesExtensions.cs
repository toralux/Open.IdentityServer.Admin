// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Resources;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Services;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Interfaces;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Shared.Configuration.Schema;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Shared.Extensions;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Extensions
{
    public static class AdminServicesExtensions
    {
        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUser>(
            this IServiceCollection services)
            where TIdentityDbContext : IdentityDbContext<TUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>, IdentityUserPasskey<string>>
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TUser : IdentityUser
        {
            return services.AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, UserDto<string>, RoleDto<string>,
                TUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>, IdentityUserPasskey<string>,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>();
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey,
            TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto>(
                this IServiceCollection services)
            where TAdminDbContext :
            IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey>,
            IAdminPersistedGrantDbContext
            where TUserDto : UserDto<TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TUserPasskey : IdentityUserPasskey<TKey>
            where TRoleDto : RoleDto<TKey>
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
            if (typeof(TUserDtoKey) != typeof(TKey) || typeof(TRoleDtoKey) != typeof(TKey))
            {
                throw new InvalidOperationException(
                    $"Generic key parameters must match. " +
                    $"{nameof(TUserDtoKey)} and {nameof(TRoleDtoKey)} must be the same type as {nameof(TKey)}.");
            }

            return services.AddAdminAspNetIdentityServices<TAdminDbContext, TAdminDbContext, TUserDto, TRoleDto,
                TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>();
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
                        this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey>
            where TUserDto : UserDto<TKey>
            where TUser : IdentityUser<TKey>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>
            where TUserRole : IdentityUserRole<TKey>
            where TUserLogin : IdentityUserLogin<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>
            where TUserToken : IdentityUserToken<TKey>
            where TUserPasskey : IdentityUserPasskey<TKey>
            where TRoleDto : RoleDto<TKey>
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
            //Repositories
            services.AddTransient<IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityRepository<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey>>();
            services.AddTransient<IPersistedGrantAspNetIdentityRepository, PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TUserPasskey>>();
            services.AddTransient<IDashboardIdentityRepository, DashboardIdentityRepository<TUser, TKey, TRole>>();

            //Services
            services.AddTransient<IIdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
                    TUserClaimDto, TRoleClaimDto>,
                IdentityDataMapper<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserLogin, TRoleClaim,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto, TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
                    TUserClaimDto, TRoleClaimDto>>();
            services.AddTransient<IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>, 
                IdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>>();
            services.AddTransient<IPersistedGrantAspNetIdentityService, PersistedGrantAspNetIdentityService>();
            services.AddTransient<IDashboardIdentityService, DashboardIdentityService>();
            
            //Resources
            services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
            services.AddScoped<IPersistedGrantAspNetIdentityServiceResources, PersistedGrantAspNetIdentityServiceResources>();

            return services;
        }

        public static void AddConfigureAdminAspNetIdentitySchema(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureAdminAspNetIdentitySchema(options =>
            {
                configuration.GetSection(nameof(IdentityTableConfiguration)).Bind(options);
            });
        }

        public static IServiceCollection AddIdentityUserMappingCustomizer<TUserDto, TUser, TCustomizer>(
            this IServiceCollection services)
            where TCustomizer : class, IIdentityUserMappingCustomizer<TUserDto, TUser>
        {
            services.AddTransient<IIdentityUserMappingCustomizer<TUserDto, TUser>, TCustomizer>();
            return services;
        }

        public static IServiceCollection AddIdentityRoleMappingCustomizer<TRoleDto, TRole, TCustomizer>(
            this IServiceCollection services)
            where TCustomizer : class, IIdentityRoleMappingCustomizer<TRoleDto, TRole>
        {
            services.AddTransient<IIdentityRoleMappingCustomizer<TRoleDto, TRole>, TCustomizer>();
            return services;
        }
    }
}
