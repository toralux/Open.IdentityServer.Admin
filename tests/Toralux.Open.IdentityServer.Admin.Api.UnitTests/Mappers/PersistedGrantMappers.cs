using System;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant;
using Toralux.Open.IdentityServer.Admin.UI.Api.Dtos.PersistedGrants;
using Toralux.Open.IdentityServer.Admin.UI.Api.Mappers;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.Api.UnitTests.Mappers
{
    public class PersistedGrantMappers
    {
        [Fact]
        public void CanMapPersistedGrantDtoToPersistedGrantSubjectsApiDto()
        {
            var persistedGrantDto = PersistedGrantDtoMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var persistedGrantsDto = new PersistedGrantsDto();
            
            persistedGrantsDto.PersistedGrants.Add(persistedGrantDto);

            var persistedGrantSubjectsApiDto = persistedGrantsDto.ToPersistedGrantSubjectsApiDto();

            persistedGrantSubjectsApiDto.Should().BeEquivalentTo(persistedGrantsDto, options => options.Excluding(x=> x.SubjectId));
        }

        [Fact]
        public void CanMapPersistedGrantDtoToPersistedGrantsApiDto()
        {
            var persistedGrantDto = PersistedGrantDtoMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var persistedGrantsDto = new PersistedGrantsDto();

            persistedGrantsDto.PersistedGrants.Add(persistedGrantDto);

            var persistedGrantsApiDto = persistedGrantsDto.ToPersistedGrantsApiDto();

            persistedGrantsApiDto.Should().BeEquivalentTo(persistedGrantsDto, options => options.Excluding(x => x.SubjectId));
        }

        [Fact]
        public void CanMapPersistedGrantDtoToPersistedGrantApiDto()
        {
            var persistedGrantDto = PersistedGrantDtoMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var persistedGrantApiDto = persistedGrantDto.ToPersistedGrantApiDto();

            persistedGrantApiDto.Should().BeEquivalentTo(persistedGrantDto, options => options.Excluding(x => x.SubjectId));
        }
    }
}