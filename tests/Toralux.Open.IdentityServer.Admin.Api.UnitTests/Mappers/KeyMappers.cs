using System;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mocks;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Key;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.Key;
using Toralux.Open.IdentityServer.Admin.UI.Api.Mappers;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mappers
{
    public class KeyMappers
    {
        [Fact]
        public void CanMapKeyDtoToKayApiDto()
        {
            var keyDto = KeyDtoMock.GenerateRandomKey(Guid.NewGuid().ToString());

            var keyApi = keyDto.ToKeyApiDto();

            keyApi.Should().BeEquivalentTo(keyDto);
        }

        [Fact]
        public void CanMapKeyApiDtoToKeyDto()
        {
            var keyApiDto = KeyApiDtoMock.GenerateRandomKey(Guid.NewGuid().ToString());

            var keyDto = keyApiDto.ToKeyDto();

            keyDto.Should().BeEquivalentTo(keyApiDto);
        }
    }
}