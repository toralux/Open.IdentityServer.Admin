// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.ConfigurationRules;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Resources;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.ConfigurationRules;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Repositories;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Repositories.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Extensions
{
    public static class AdminServicesExtensions
    {
        public static IServiceCollection AddAdminServices<TAdminDbContext>(
            this IServiceCollection services)
            where TAdminDbContext : DbContext, IAdminPersistedGrantDbContext, IAdminConfigurationDbContext, IAdminLogDbContext, IAdminConfigurationStoreDbContext
        {

            return services.AddAdminServices<TAdminDbContext, TAdminDbContext, TAdminDbContext, TAdminDbContext>();
        }

        public static IServiceCollection AddAdminServices<TConfigurationDbContext, TPersistedGrantDbContext, TLogDbContext, TAdminConfigurationDbContext>(this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TConfigurationDbContext : DbContext, IAdminConfigurationDbContext
            where TLogDbContext : DbContext, IAdminLogDbContext
            where TAdminConfigurationDbContext : DbContext, IAdminConfigurationStoreDbContext
        {
            //Repositories
            services.AddTransient<IClientRepository, ClientRepository<TConfigurationDbContext>>();
            services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IApiResourceRepository, ApiResourceRepository<TConfigurationDbContext>>();
            services.AddTransient<IApiScopeRepository, ApiScopeRepository<TConfigurationDbContext>>();
            services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository<TPersistedGrantDbContext>>();
            services.AddTransient<ILogRepository, LogRepository<TLogDbContext>>();
            services.AddTransient<IDashboardRepository, DashboardRepository<TConfigurationDbContext>>();
            services.AddTransient<IConfigurationIssuesRepository, ConfigurationIssuesRepository<TConfigurationDbContext, TAdminConfigurationDbContext>>();

            // Configuration Rules
            services.AddTransient<IConfigurationRulesRepository, ConfigurationRulesRepository<TAdminConfigurationDbContext>>();
            services.AddScoped<IConfigurationRuleValidatorFactory, ConfigurationRuleValidatorFactory>();
            services.AddScoped<IConfigurationRuleMetadataProvider, ConfigurationRuleMetadataProvider>();

            //Services
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IApiResourceService, ApiResourceService>();
            services.AddTransient<IApiScopeService, ApiScopeService>();
            services.AddTransient<IIdentityResourceService, IdentityResourceService>();
            services.AddTransient<IPersistedGrantService, PersistedGrantService>();
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IConfigurationIssuesService, ConfigurationIssuesService>();
            services.AddTransient<IConfigurationRulesService, ConfigurationRulesService>();

            //Resources
            services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
            services.AddScoped<IApiScopeServiceResources, ApiScopeServiceResources>();
            services.AddScoped<IClientServiceResources, ClientServiceResources>();
            services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
            services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();

            return services;
        }
    }
}
