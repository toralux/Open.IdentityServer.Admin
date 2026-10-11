// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Open.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Moq;
using Skoruba.AuditLogging.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.PersistedGrant;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Resources;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Entities;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Services
{
    public class PersistedGrantServiceTests
    {
        private static readonly Faker Faker = new();

        [Fact]
        public async Task GetPersistedGrantsByUsersAsync_ReturnsPagedPersistedGrantSubjects()
        {
            var pagedPersistedGrants = new PagedList<PersistedGrantDataView>
            {
                TotalCount = 2,
                PageSize = 10
            };
            pagedPersistedGrants.Data.Add(new PersistedGrantDataView
            {
                SubjectId = Faker.Random.Guid().ToString(),
                SubjectName = Faker.Name.FullName()
            });
            pagedPersistedGrants.Data.Add(new PersistedGrantDataView
            {
                SubjectId = Faker.Random.Guid().ToString(),
                SubjectName = Faker.Name.FullName()
            });

            var repositoryMock = new Mock<IPersistedGrantRepository>();
            repositoryMock
                .Setup(x => x.GetPersistedGrantsByUsersAsync("user", 2, 10))
                .ReturnsAsync(pagedPersistedGrants);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<PersistedGrantsByUsersRequestedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IPersistedGrantServiceResources>();
            IPersistedGrantService service = new PersistedGrantService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.GetPersistedGrantsByUsersAsync("user", 2, 10);

            result.TotalCount.Should().Be(pagedPersistedGrants.TotalCount);
            result.PageSize.Should().Be(pagedPersistedGrants.PageSize);
            result.PersistedGrants.Should().HaveCount(pagedPersistedGrants.Data.Count);
            result.PersistedGrants.Select(x => x.SubjectId).Should().BeEquivalentTo(pagedPersistedGrants.Data.Select(x => x.SubjectId));
        }

        [Fact]
        public async Task GetPersistedGrantsByUserAsync_ReturnsPagedPersistedGrants()
        {
            var subjectId = Faker.Random.Guid().ToString();
            var pagedPersistedGrants = new PagedList<PersistedGrant>
            {
                TotalCount = 2,
                PageSize = 5
            };
            pagedPersistedGrants.Data.Add(PersistedGrantMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString(), subjectId));
            pagedPersistedGrants.Data.Add(PersistedGrantMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString(), subjectId));

            var repositoryMock = new Mock<IPersistedGrantRepository>();
            repositoryMock
                .Setup(x => x.ExistsPersistedGrantsAsync(subjectId))
                .ReturnsAsync(true);
            repositoryMock
                .Setup(x => x.GetPersistedGrantsByUserAsync(subjectId, 1, 5))
                .ReturnsAsync(pagedPersistedGrants);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<PersistedGrantsByUserRequestedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IPersistedGrantServiceResources>();
            IPersistedGrantService service = new PersistedGrantService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.GetPersistedGrantsByUserAsync(subjectId, 1, 5);

            result.TotalCount.Should().Be(pagedPersistedGrants.TotalCount);
            result.PageSize.Should().Be(pagedPersistedGrants.PageSize);
            result.PersistedGrants.Should().HaveCount(pagedPersistedGrants.Data.Count);
            result.PersistedGrants.Select(x => x.Key).Should().BeEquivalentTo(pagedPersistedGrants.Data.Select(x => x.Key));
        }

        [Fact]
        public async Task GetPersistedGrantAsync_AuditEvent_DoesNotContainPersistedGrantData()
        {
            var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString());
            var repositoryMock = new Mock<IPersistedGrantRepository>();
            repositoryMock.Setup(x => x.GetPersistedGrantAsync(persistedGrant.Key)).ReturnsAsync(persistedGrant);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<PersistedGrantRequestedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IPersistedGrantServiceResources>();
            IPersistedGrantService service = new PersistedGrantService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.GetPersistedGrantAsync(persistedGrant.Key);

            result.Data.Should().Be(persistedGrant.Data);
            result.SessionId.Should().Be(persistedGrant.SessionId);
            auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<PersistedGrantRequestedEvent>(e =>
                e.PersistedGrant.Data == null &&
                e.PersistedGrant.SessionId == null)), Times.Once);
        }

        [Fact]
        public async Task DeletePersistedGrantAsync_ReturnsDeletedPersistedGrantCount()
        {
            var persistedGrantKey = Guid.NewGuid().ToString();

            var repositoryMock = new Mock<IPersistedGrantRepository>();
            repositoryMock
                .Setup(x => x.DeletePersistedGrantAsync(persistedGrantKey))
                .ReturnsAsync(1);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<PersistedGrantDeletedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IPersistedGrantServiceResources>();
            IPersistedGrantService service = new PersistedGrantService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.DeletePersistedGrantAsync(persistedGrantKey);

            result.Should().Be(1);
        }

        [Fact]
        public async Task DeletePersistedGrantsAsync_ReturnsDeletedPersistedGrantCount()
        {
            var subjectId = Faker.Random.Guid().ToString();

            var repositoryMock = new Mock<IPersistedGrantRepository>();
            repositoryMock
                .Setup(x => x.DeletePersistedGrantsAsync(subjectId))
                .ReturnsAsync(2);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<PersistedGrantsDeletedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IPersistedGrantServiceResources>();
            IPersistedGrantService service = new PersistedGrantService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.DeletePersistedGrantsAsync(subjectId);

            result.Should().Be(2);
        }
    }
}
