// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Linq;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers.Converters;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Mappers
{
	public class ApiResourceMappers
	{
		[Fact]
		public void CanMapApiResourceToModel()
		{
			//Generate entity
			var apiResource = ApiResourceMock.GenerateRandomApiResource(1);

			//Try map to DTO
			var apiResourceDto = apiResource.ToModel();

			//Assert
			apiResourceDto.Should().NotBeNull();

            apiResourceDto.Should().BeEquivalentTo(apiResource, options =>
				options.Excluding(o => o.Secrets)
					   .Excluding(o => o.Scopes)
					   .Excluding(o => o.Properties)
					   .Excluding(o => o.Created)
					   .Excluding(o => o.Updated)
					   .Excluding(o => o.LastAccessed)
					   .Excluding(o => o.NonEditable)
                       .Excluding(o => o.AllowedAccessTokenSigningAlgorithms)
					   .Excluding(o => o.UserClaims));

			//Assert collection
            apiResourceDto.UserClaims.Should().BeEquivalentTo(apiResource.UserClaims.Select(x => x.Type));

            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(apiResource.AllowedAccessTokenSigningAlgorithms, null);
            apiResourceDto.AllowedAccessTokenSigningAlgorithms.Should().BeEquivalentTo(allowedAlgList);
		}

		[Fact]
		public void CanMapApiResourceDtoToEntity()
		{
			//Generate DTO
			var apiResourceDto = ApiResourceDtoMock.GenerateRandomApiResource(1);

			//Try map to entity
			var apiResource = apiResourceDto.ToEntity();

			apiResource.Should().NotBeNull();

            apiResourceDto.Should().BeEquivalentTo(apiResource, options =>
				options.Excluding(o => o.Secrets)
					.Excluding(o => o.Scopes)
					.Excluding(o => o.Properties)
					.Excluding(o => o.Created)
					.Excluding(o => o.Updated)
					.Excluding(o => o.LastAccessed)
					.Excluding(o => o.NonEditable)
                    .Excluding(o => o.AllowedAccessTokenSigningAlgorithms)
					.Excluding(o => o.UserClaims));

			//Assert collection
            apiResourceDto.UserClaims.Should().BeEquivalentTo(apiResource.UserClaims.Select(x => x.Type));
            var allowedAlgList = AllowedSigningAlgorithmsConverter.Converter.Convert(apiResource.AllowedAccessTokenSigningAlgorithms, null);
            apiResourceDto.AllowedAccessTokenSigningAlgorithms.Should().BeEquivalentTo(allowedAlgList);
		}

		[Fact]
		public void CanMapApiScopeToModel()
		{
            //Generate DTO
            var apiScopeDto = ApiScopeMock.GenerateRandomApiScope(1);

            //Try map to entity
            var apiScope = apiScopeDto.ToModel();

            apiScope.Should().NotBeNull();

            apiScopeDto.Should().BeEquivalentTo(apiScope, options =>
                options.Excluding(o => o.UserClaims)
                    .Excluding(o => o.ApiScopeProperties)
					.Excluding(o => o.UserClaimsItems));

			//Assert collection
            apiScope.UserClaims.Should().BeEquivalentTo(apiScopeDto.UserClaims.Select(x => x.Type));
            apiScope.Id.Should().Be(apiScopeDto.Id);
		}

		[Fact]
		public void CanMapApiScopeDtoToEntity()
		{
			//Generate DTO
			var apiScopeDto = ApiScopeDtoMock.GenerateRandomApiScope(1);

			//Try map to entity
			var apiScope = apiScopeDto.ToEntity();

			apiScope.Should().NotBeNull();

            apiScopeDto.Should().BeEquivalentTo(apiScope, options =>
				options.Excluding(o => o.UserClaims)
                       .Excluding(o => o.Properties)
                       .Excluding(o => o.Updated)
                       .Excluding(o => o.LastAccessed)
                       .Excluding(o => o.NonEditable)
                       .Excluding(o => o.Created)
					   .Excluding(o => o.Id));

			//Assert collection
            apiScopeDto.UserClaims.Should().BeEquivalentTo(apiScope.UserClaims.Select(x => x.Type));
			apiScope.Id.Should().Be(apiScopeDto.Id);
		}

		[Fact]
		public void CanMapApiSecretToModel()
		{
			//Generate entity
			var apiSecret = ApiResourceMock.GenerateRandomApiSecret(1);

			//Try map to DTO
			var apiSecretsDto = apiSecret.ToModel();

			//Assert
			apiSecretsDto.Should().NotBeNull();

            apiSecretsDto.Should().BeEquivalentTo(apiSecret, options =>
				options.Excluding(o => o.ApiResource)
					.Excluding(o => o.Created)
					.Excluding(o => o.Id));

			apiSecret.Id.Should().Be(apiSecretsDto.ApiSecretId);
		}

		[Fact]
		public void CanMapApiSecretDtoToEntity()
		{
			//Generate DTO
			var apiSecretsDto = ApiResourceDtoMock.GenerateRandomApiSecret(1, 1);

			//Try map to entity
			var apiSecret = apiSecretsDto.ToEntity();

			apiSecret.Should().NotBeNull();

            apiSecretsDto.Should().BeEquivalentTo(apiSecret, options =>
				options.Excluding(o => o.ApiResource)
					.Excluding(o => o.Created)
					.Excluding(o => o.Id));

			apiSecret.Id.Should().Be(apiSecretsDto.ApiSecretId);
		}

		[Fact]
		public void CanMapApiResourcePropertyToModel()
		{
			var apiResourceProperty = ApiResourceMock.GenerateRandomApiResourceProperty(1);

			var apiResourcePropertiesDto = apiResourceProperty.ToModel();

			apiResourcePropertiesDto.Should().NotBeNull();
			apiResourceProperty.Id.Should().Be(apiResourcePropertiesDto.ApiResourcePropertyId);
			apiResourcePropertiesDto.Key.Should().Be(apiResourceProperty.Key);
			apiResourcePropertiesDto.Value.Should().Be(apiResourceProperty.Value);
		}

		[Fact]
		public void CanMapApiResourcePropertyDtoToEntity()
		{
			var apiResourcePropertiesDto = ApiResourceDtoMock.GenerateRandomApiResourceProperty(1, 1);

			var apiResourceProperty = apiResourcePropertiesDto.ToEntity();

			apiResourceProperty.Should().NotBeNull();
			apiResourcePropertiesDto.ApiResourcePropertyId.Should().Be(apiResourceProperty.Id);
			apiResourceProperty.Key.Should().Be(apiResourcePropertiesDto.Key);
			apiResourceProperty.Value.Should().Be(apiResourcePropertiesDto.Value);
		}

		[Fact]
		public void CanMapPagedApiResourcesToModel()
		{
			var resources = new PagedList<ApiResource>
			{
				TotalCount = 10,
				PageSize = 5
			};
			resources.Data.Add(ApiResourceMock.GenerateRandomApiResource(1));
			resources.Data.Add(ApiResourceMock.GenerateRandomApiResource(2));

			var resourcesDto = resources.ToModel();

			resourcesDto.Should().NotBeNull();
			resourcesDto.TotalCount.Should().Be(resources.TotalCount);
			resourcesDto.PageSize.Should().Be(resources.PageSize);
			resourcesDto.ApiResources.Should().HaveCount(resources.Data.Count);
			resourcesDto.ApiResources.Select(x => x.Name).Should().BeEquivalentTo(resources.Data.Select(x => x.Name));
		}

		[Fact]
		public void CanMapPagedApiSecretsToModel()
		{
			var secrets = new PagedList<ApiResourceSecret>
			{
				TotalCount = 6,
				PageSize = 3
			};
			secrets.Data.Add(ApiResourceMock.GenerateRandomApiSecret(1));
			secrets.Data.Add(ApiResourceMock.GenerateRandomApiSecret(2));

			var secretsDto = secrets.ToModel();

			secretsDto.Should().NotBeNull();
			secretsDto.TotalCount.Should().Be(secrets.TotalCount);
			secretsDto.PageSize.Should().Be(secrets.PageSize);
			secretsDto.ApiSecrets.Should().HaveCount(secrets.Data.Count);
		}

		[Fact]
		public void CanMapPagedApiResourcePropertiesToModel()
		{
			var properties = new PagedList<ApiResourceProperty>
			{
				TotalCount = 4,
				PageSize = 2
			};
			properties.Data.Add(ApiResourceMock.GenerateRandomApiResourceProperty(1));
			properties.Data.Add(ApiResourceMock.GenerateRandomApiResourceProperty(2));

			var propertiesDto = properties.ToModel();

			propertiesDto.Should().NotBeNull();
			propertiesDto.TotalCount.Should().Be(properties.TotalCount);
			propertiesDto.PageSize.Should().Be(properties.PageSize);
			propertiesDto.ApiResourceProperties.Should().HaveCount(properties.Data.Count);
		}
	}
}