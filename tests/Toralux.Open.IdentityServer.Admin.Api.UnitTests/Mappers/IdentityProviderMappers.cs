using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mocks;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.IdentityProvider;
using Toralux.Open.IdentityServer.Admin.UI.Api.Mappers;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mappers
{
    public class IdentityProviderMappers
    {
        [Fact]
        public void CanMapIdentityProviderApiDtoToIdentityProviderDto()
        {
            var IdentityProviderApiDto = IdentityProviderApiDtoMock.GenerateRandomIdentityProvider(1);

            var IdentityProviderDto = IdentityProviderApiDto.ToIdentityProviderDto();

            IdentityProviderDto.Should().NotBeNull();

            IdentityProviderDto.Should().BeEquivalentTo(IdentityProviderApiDto, options => options
                .Excluding(x => x.IdentityProviderProperties));

            IdentityProviderDto.Properties.Values.Should().BeEquivalentTo(
                IdentityProviderApiDto.IdentityProviderProperties.Select(p => new IdentityProviderPropertyDto
                    { Name = p.Key, Value = p.Value }));
        }

        [Fact]
        public void CanMapIdentityProviderDtoToIdentityProviderApiDto()
        {
            var IdentityProviderDto = IdentityProviderDtoMock.GenerateRandomIdentityProvider(1);

            var IdentityProviderApiDto = IdentityProviderDto.ToIdentityProviderApiDto();

            IdentityProviderApiDto.Should().BeEquivalentTo(IdentityProviderDto, options => options
                .Excluding(x => x.Properties));

            IdentityProviderApiDto.IdentityProviderProperties.Should().BeEquivalentTo(
                IdentityProviderDto.Properties.Values.ToDictionary(p=>p.Name, p=>p.Value));

        }

        [Fact]
        public void CanMapIdentityProviderDtoToIdentityProviderApiDto_WithInvalidPropertyNames()
        {
            var identityProviderDto = IdentityProviderDtoMock.GenerateRandomIdentityProvider(1);
            identityProviderDto.Properties = new Dictionary<int, IdentityProviderPropertyDto>
            {
                [0] = new IdentityProviderPropertyDto { Name = null, Value = "ignored-null-name" },
                [1] = null,
                [2] = new IdentityProviderPropertyDto { Name = "valid-name", Value = "valid-value" }
            };

            var identityProviderApiDto = identityProviderDto.ToIdentityProviderApiDto();

            identityProviderApiDto.IdentityProviderProperties.Should().ContainSingle();
            identityProviderApiDto.IdentityProviderProperties["valid-name"].Should().Be("valid-value");
        }

    }
}
