// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Threading.Tasks;
using Duende.IdentityServer.EntityFramework.Options;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Skoruba.AuditLogging.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Events.PersistedGrant;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Resources;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Services
{
    public class PersistedGrantAspNetIdentityServiceTests
    {
        public PersistedGrantAspNetIdentityServiceTests()
        {
            var identityDatabaseName = Guid.NewGuid().ToString();

            _identityDbContextOptions = new DbContextOptionsBuilder<AdminIdentityDbContext>()
                .UseInMemoryDatabase(identityDatabaseName)
                .Options;
        }

        private readonly DbContextOptions<AdminIdentityDbContext> _identityDbContextOptions;

        private IdentityServerPersistedGrantDbContext GetDbContext()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(new ConfigurationStoreOptions());
            serviceCollection.AddSingleton(new OperationalStoreOptions());

            serviceCollection.AddDbContext<IdentityServerPersistedGrantDbContext>(builder => builder.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = serviceProvider.GetService<IdentityServerPersistedGrantDbContext>();

            return context;
        }

        private IPersistedGrantAspNetIdentityRepository GetPersistedGrantRepository(AdminIdentityDbContext identityDbContext, IdentityServerPersistedGrantDbContext context)
        {
            var persistedGrantRepository = new PersistedGrantAspNetIdentityRepository<AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole, UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken, UserIdentityPasskey>(identityDbContext, context);

            return persistedGrantRepository;
        }

        private IPersistedGrantAspNetIdentityService
            GetPersistedGrantService(IPersistedGrantAspNetIdentityRepository repository, IPersistedGrantAspNetIdentityServiceResources persistedGrantServiceResources, IAuditEventLogger auditEventLogger)
        {
            var persistedGrantService = new PersistedGrantAspNetIdentityService(repository,
                persistedGrantServiceResources, auditEventLogger);

            return persistedGrantService;
        }

        [Fact]
        public async Task GetPersistedGrantAsync()
        {
            using (var context = GetDbContext())
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    var auditLogger = auditLoggerMock.Object;
                    
                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLogger);

                    var persistedGrantKey = Guid.NewGuid().ToString();
                    var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey);

                    await context.PersistedGrants.AddAsync(persistedGrant);
                    await context.SaveChangesAsync();

                    var persistedGrantAdded = await persistedGrantService.GetPersistedGrantAsync(persistedGrantKey);

                    persistedGrantAdded.Should().BeEquivalentTo(persistedGrant);
                }
            }
        }

        [Fact]
        public async Task DeletePersistedGrantAsync()
        {
            using (var context = GetDbContext())
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    var auditLogger = auditLoggerMock.Object;

                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLogger);

                    var persistedGrantKey = Guid.NewGuid().ToString();
                    var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey);

                    await context.PersistedGrants.AddAsync(persistedGrant);
                    await context.SaveChangesAsync();

                    await persistedGrantService.DeletePersistedGrantAsync(persistedGrantKey);

                    var grant = await persistedGrantRepository.GetPersistedGrantAsync(persistedGrantKey);

                    grant.Should().BeNull();
                }
            }
        }

        [Fact]
        public async Task DeletePersistedGrantsAsync()
        {
            using (var context = GetDbContext())
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    var auditLogger = auditLoggerMock.Object;

                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLogger);

                    const int subjectId = 1;

                    for (var i = 0; i < 4; i++)
                    {
                        var persistedGrantKey = Guid.NewGuid().ToString();
                        var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey, subjectId.ToString());

                        await context.PersistedGrants.AddAsync(persistedGrant);
                    }

                    await context.SaveChangesAsync();

                    await persistedGrantService.DeletePersistedGrantsAsync(subjectId.ToString());

                    var grant = await persistedGrantRepository.GetPersistedGrantsByUserAsync(subjectId.ToString());

                    grant.TotalCount.Should().Be(0);
                }
            }
        }

        [Fact]
        public async Task GetPersistedGrantAsync_AuditEvent_DoesNotContainPersistedGrantData()
        {
            using (var context = GetDbContext())
            {
                using (var identityDbContext = new AdminIdentityDbContext(_identityDbContextOptions))
                {
                    var persistedGrantRepository = GetPersistedGrantRepository(identityDbContext, context);

                    var localizerMock = new Mock<IPersistedGrantAspNetIdentityServiceResources>();
                    var localizer = localizerMock.Object;

                    var auditLoggerMock = new Mock<IAuditEventLogger>();
                    auditLoggerMock
                        .Setup(x => x.LogEventAsync(It.IsAny<PersistedGrantIdentityRequestedEvent>()))
                        .Returns(Task.CompletedTask);

                    var persistedGrantService = GetPersistedGrantService(persistedGrantRepository, localizer, auditLoggerMock.Object);

                    var persistedGrantKey = Guid.NewGuid().ToString();
                    var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey);

                    await context.PersistedGrants.AddAsync(persistedGrant);
                    await context.SaveChangesAsync();

                    var persistedGrantAdded = await persistedGrantService.GetPersistedGrantAsync(persistedGrantKey);

                    persistedGrantAdded.Data.Should().Be(persistedGrant.Data);
                    persistedGrantAdded.SessionId.Should().Be(persistedGrant.SessionId);
                    auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<PersistedGrantIdentityRequestedEvent>(e =>
                        e.PersistedGrant.Data == null &&
                        e.PersistedGrant.SessionId == null)), Times.Once);
                }
            }
        }
    }
}
