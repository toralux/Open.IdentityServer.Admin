using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mocks;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Configuration;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.IdentityResources;
using Toralux.Open.IdentityServer.Admin.UI.Api.Mappers;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mappers
{
    public class IdentityResourceMappers
    {
        [Fact]
        public void CanMapIdentityResourceApiDtoToIdentityResourceDto()
        {
            var identityResourceApiDto = IdentityResourceApiDtoMock.GenerateRandomIdentityResource(1);

            var identityResourceDto = identityResourceApiDto.ToIdentityResourceDto();

            identityResourceDto.Should().NotBeNull();

            identityResourceDto.Should().BeEquivalentTo(identityResourceApiDto);
        }

        [Fact]
        public void CanMapIdentityResourceDtoToIdentityResourceApiDto()
        {
            var identityResourceDto = IdentityResourceDtoMock.GenerateRandomIdentityResource(1);

            var identityResourceApiDto = identityResourceDto.ToIdentityResourceApiDto();

            identityResourceApiDto.Should().BeEquivalentTo(identityResourceDto, options => options
                .Excluding(x => x.UserClaimsItems));
        }

        [Fact]
        public void CanMapIdentityResourcePropertyApiDtoToIdentityResourcePropertyDto()
        {
            var identityResourcePropertyApiDto = IdentityResourceApiDtoMock.GenerateRandomIdentityResourceProperty(1);

            var identityResourcePropertiesDto = identityResourcePropertyApiDto.ToIdentityResourcePropertiesDto();

            identityResourcePropertyApiDto.Id.Should().Be(identityResourcePropertiesDto.IdentityResourcePropertyId);

            identityResourcePropertiesDto.Should().BeEquivalentTo(identityResourcePropertyApiDto, options => options.Excluding(x => x.Id));
        }

        [Fact]
        public void CanMapIdentityResourcePropertyDtoToIdentityResourcePropertyApiDto()
        {
            var identityResourcePropertyDto = IdentityResourceDtoMock.GenerateRandomIdentityResourceProperty(1, 1);

            var identityResourcePropertyApiDto = identityResourcePropertyDto.ToIdentityResourcePropertyApiDto();

            identityResourcePropertyDto.IdentityResourcePropertyId.Should().Be(identityResourcePropertyApiDto.Id);

            identityResourcePropertyApiDto.Should().BeEquivalentTo(identityResourcePropertyDto, options =>
                options.Excluding(x => x.IdentityResourceId)
                    .Excluding(x => x.IdentityResourceName)
                    .Excluding(x => x.PageSize)
                    .Excluding(x => x.TotalCount)
                    .Excluding(x => x.IdentityResourcePropertyId)
                    .Excluding(x => x.IdentityResourceProperties));
        }
    }
}