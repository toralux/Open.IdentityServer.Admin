// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Mappers
{
    public class IdentityResourceMappers
    {
        [Fact]
        public void CanMapIdentityResourceToModel()
        {
            //Generate entity
            var identityResource = IdentityResourceMock.GenerateRandomIdentityResource(1);

            //Try map to DTO
            var identityResourceDto = identityResource.ToModel();

            //Assert
            identityResourceDto.Should().NotBeNull();

            identityResourceDto.Should().BeEquivalentTo(identityResource, options =>
                options.Excluding(o => o.UserClaims)
		            .Excluding(o => o.Properties)
		            .Excluding(o => o.Created)
		            .Excluding(o => o.Updated)
		            .Excluding(o => o.NonEditable));

            //Assert collection
            identityResourceDto.UserClaims.Should().BeEquivalentTo(identityResource.UserClaims.Select(x => x.Type));
        }

        [Fact]
        public void CanMapIdentityResourceDtoToEntity()
        {
            //Generate DTO
            var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(1);

            //Try map to entity
            var identityResource = identityResourceDto.ToEntity();

            identityResource.Should().NotBeNull();

            identityResourceDto.Should().BeEquivalentTo(identityResource, options =>
                options.Excluding(o => o.UserClaims)
				.Excluding(o => o.Properties)
		            .Excluding(o => o.Created)
		            .Excluding(o => o.Updated)
		            .Excluding(o => o.NonEditable));

            //Assert collection
            identityResourceDto.UserClaims.Should().BeEquivalentTo(identityResource.UserClaims.Select(x => x.Type));
        }

        [Fact]
        public void CanMapIdentityResourcePropertyToModel()
        {
            var identityResourceProperty = IdentityResourceMock.GenerateRandomIdentityResourceProperty(1);

            var identityResourcePropertiesDto = identityResourceProperty.ToModel();

            identityResourcePropertiesDto.Should().NotBeNull();
            identityResourceProperty.Id.Should().Be(identityResourcePropertiesDto.IdentityResourcePropertyId);
            identityResourcePropertiesDto.Key.Should().Be(identityResourceProperty.Key);
            identityResourcePropertiesDto.Value.Should().Be(identityResourceProperty.Value);
        }

        [Fact]
        public void CanMapIdentityResourcePropertyDtoToEntity()
        {
            var identityResourcePropertiesDto = IdentityResourceDtoMock.GenerateRandomIdentityResourceProperty(1, 1);

            var identityResourceProperty = identityResourcePropertiesDto.ToEntity();

            identityResourceProperty.Should().NotBeNull();
            identityResourcePropertiesDto.IdentityResourcePropertyId.Should().Be(identityResourceProperty.Id);
            identityResourceProperty.Key.Should().Be(identityResourcePropertiesDto.Key);
            identityResourceProperty.Value.Should().Be(identityResourcePropertiesDto.Value);
        }

        [Fact]
        public void CanMapPagedIdentityResourcesToModel()
        {
            var resources = new PagedList<IdentityResource>
            {
                TotalCount = 8,
                PageSize = 4
            };
            resources.Data.Add(IdentityResourceMock.GenerateRandomIdentityResource(1));
            resources.Data.Add(IdentityResourceMock.GenerateRandomIdentityResource(2));

            var resourcesDto = resources.ToModel();

            resourcesDto.Should().NotBeNull();
            resourcesDto.TotalCount.Should().Be(resources.TotalCount);
            resourcesDto.PageSize.Should().Be(resources.PageSize);
            resourcesDto.IdentityResources.Should().HaveCount(resources.Data.Count);
            resourcesDto.IdentityResources.Select(x => x.Name).Should().BeEquivalentTo(resources.Data.Select(x => x.Name));
        }

        [Fact]
        public void CanMapPagedIdentityResourcePropertiesToModel()
        {
            var properties = new PagedList<IdentityResourceProperty>
            {
                TotalCount = 4,
                PageSize = 2
            };
            properties.Data.Add(IdentityResourceMock.GenerateRandomIdentityResourceProperty(1));
            properties.Data.Add(IdentityResourceMock.GenerateRandomIdentityResourceProperty(2));

            var propertiesDto = properties.ToModel();

            propertiesDto.Should().NotBeNull();
            propertiesDto.TotalCount.Should().Be(properties.TotalCount);
            propertiesDto.PageSize.Should().Be(properties.PageSize);
            propertiesDto.IdentityResourceProperties.Should().HaveCount(properties.Data.Count);
        }
    }
}
