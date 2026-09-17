// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using Open.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Moq;
using Skoruba.AuditLogging.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Events.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Resources;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Services
{
    public class IdentityProviderServiceTests
    {
        private static readonly Faker Faker = new();

        [Fact]
        public async Task GetIdentityProvidersAsync_ReturnsPagedIdentityProviders()
        {
            var identityProviders = new PagedList<IdentityProvider>
            {
                TotalCount = 2,
                PageSize = 10
            };
            identityProviders.Data.Add(IdentityProviderMock.GenerateRandomIdentityProvider(1));
            identityProviders.Data.Add(IdentityProviderMock.GenerateRandomIdentityProvider(2));

            var repositoryMock = new Mock<IIdentityProviderRepository>();
            repositoryMock
                .Setup(x => x.GetIdentityProvidersAsync("oidc", 2, 10))
                .ReturnsAsync(identityProviders);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<IdentityProvidersRequestedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IIdentityProviderServiceResources>();
            IIdentityProviderService service = new IdentityProviderService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.GetIdentityProvidersAsync("oidc", 2, 10);

            result.TotalCount.Should().Be(identityProviders.TotalCount);
            result.PageSize.Should().Be(identityProviders.PageSize);
            result.IdentityProviders.Should().HaveCount(identityProviders.Data.Count);
            result.IdentityProviders.Select(x => x.Id).Should().BeEquivalentTo(identityProviders.Data.Select(x => x.Id));
        }

        [Fact]
        public async Task GetIdentityProviderAsync_AuditEvent_DoesNotContainPropertyValues()
        {
            var identityProvider = IdentityProviderMock.GenerateRandomIdentityProvider(1);
            var publicClientId = Faker.Random.Guid().ToString();
            var clientSecret = Faker.Random.AlphaNumeric(40);
            identityProvider.Properties = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["client_id"] = publicClientId,
                ["client_secret"] = clientSecret
            });

            var repositoryMock = new Mock<IIdentityProviderRepository>();
            repositoryMock
                .Setup(x => x.GetIdentityProviderAsync(1))
                .ReturnsAsync(identityProvider);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<IdentityProviderRequestedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IIdentityProviderServiceResources>();
            IIdentityProviderService service = new IdentityProviderService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.GetIdentityProviderAsync(1);

            result.Properties.Values.Should().Contain(x => x.Value == clientSecret);
            result.Properties.Values.Should().Contain(x => x.Value == publicClientId);
            auditLoggerMock.Verify(x => x.LogEventAsync(It.Is<IdentityProviderRequestedEvent>(e =>
                e.IdentityProvider.Properties.Values.All(p => p.Value == null))), Times.Once);
        }

        [Fact]
        public async Task AddIdentityProviderAsync_ReturnsSavedIdentityProviderId()
        {
            var identityProvider = IdentityProviderDtoMock.GenerateRandomIdentityProvider(1);

            var repositoryMock = new Mock<IIdentityProviderRepository>();
            repositoryMock
                .Setup(x => x.CanInsertIdentityProviderAsync(It.IsAny<IdentityProvider>()))
                .ReturnsAsync(true);
            repositoryMock
                .Setup(x => x.AddIdentityProviderAsync(It.IsAny<IdentityProvider>()))
                .ReturnsAsync(identityProvider.Id);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<IdentityProviderAddedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IIdentityProviderServiceResources>();
            IIdentityProviderService service = new IdentityProviderService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.AddIdentityProviderAsync(identityProvider);

            result.Should().Be(identityProvider.Id);
            repositoryMock.Verify(x => x.AddIdentityProviderAsync(It.Is<IdentityProvider>(e =>
                e.Scheme == identityProvider.Scheme &&
                e.Type == identityProvider.Type &&
                e.DisplayName == identityProvider.DisplayName)), Times.Once);
        }

        [Fact]
        public async Task UpdateIdentityProviderAsync_InitializesMissingProperties()
        {
            var originalIdentityProvider = IdentityProviderMock.GenerateRandomIdentityProvider(1);
            var updatedIdentityProvider = IdentityProviderDtoMock.GenerateRandomIdentityProvider(1);
            updatedIdentityProvider.Properties = null;

            IdentityProvider savedEntity = null;

            var repositoryMock = new Mock<IIdentityProviderRepository>();
            repositoryMock
                .Setup(x => x.CanInsertIdentityProviderAsync(It.IsAny<IdentityProvider>()))
                .ReturnsAsync(true);
            repositoryMock
                .Setup(x => x.GetIdentityProviderAsync(updatedIdentityProvider.Id))
                .ReturnsAsync(originalIdentityProvider);
            repositoryMock
                .Setup(x => x.UpdateIdentityProviderAsync(It.IsAny<IdentityProvider>()))
                .Callback<IdentityProvider>(entity => savedEntity = entity)
                .ReturnsAsync(updatedIdentityProvider.Id);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<IdentityProviderRequestedEvent>()))
                .Returns(Task.CompletedTask);
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<IdentityProviderUpdatedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IIdentityProviderServiceResources>();
            IIdentityProviderService service = new IdentityProviderService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.UpdateIdentityProviderAsync(updatedIdentityProvider);

            result.Should().Be(updatedIdentityProvider.Id);
            savedEntity.Should().NotBeNull();
            savedEntity.Properties.Should().Be("{}");
        }

        [Fact]
        public async Task DeleteIdentityProviderAsync_ReturnsDeletedIdentityProviderId()
        {
            var identityProvider = IdentityProviderDtoMock.GenerateRandomIdentityProvider(1);

            var repositoryMock = new Mock<IIdentityProviderRepository>();
            repositoryMock
                .Setup(x => x.DeleteIdentityProviderAsync(It.IsAny<IdentityProvider>()))
                .ReturnsAsync(identityProvider.Id);

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            auditLoggerMock
                .Setup(x => x.LogEventAsync(It.IsAny<IdentityProviderDeletedEvent>()))
                .Returns(Task.CompletedTask);

            var resourcesMock = new Mock<IIdentityProviderServiceResources>();
            IIdentityProviderService service = new IdentityProviderService(repositoryMock.Object, resourcesMock.Object, auditLoggerMock.Object);

            var result = await service.DeleteIdentityProviderAsync(identityProvider);

            result.Should().Be(identityProvider.Id);
            repositoryMock.Verify(x => x.DeleteIdentityProviderAsync(It.Is<IdentityProvider>(e =>
                e.Id == identityProvider.Id &&
                e.Scheme == identityProvider.Scheme)), Times.Once);
        }
    }
}
