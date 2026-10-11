using System;
using System.Linq;
using Open.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Mappers
{
    public class KeyMappers
    {
        [Fact]
        public void CanMapKeyToKeyDto()
        {
            var key = KeyMock.GenerateRandomKey(Guid.NewGuid().ToString());

            var keyDto = key.ToModel();

            keyDto.Should().BeEquivalentTo(key, options => 
                options.Excluding(x=> x.DataProtected)
                    .Excluding(x=> x.Data));
        }

        [Fact]
        public void CanMapPagedKeysToModel()
        {
            var keys = new PagedList<Key>
            {
                TotalCount = 5,
                PageSize = 3
            };
            keys.Data.Add(KeyMock.GenerateRandomKey(Guid.NewGuid().ToString()));
            keys.Data.Add(KeyMock.GenerateRandomKey(Guid.NewGuid().ToString()));

            var keysDto = keys.ToModel();

            keysDto.Should().NotBeNull();
            keysDto.TotalCount.Should().Be(keys.TotalCount);
            keysDto.PageSize.Should().Be(keys.PageSize);
            keysDto.Keys.Should().HaveCount(keys.Data.Count);
            keysDto.Keys.Select(x => x.Id).Should().BeEquivalentTo(keys.Data.Select(x => x.Id));
        }
    }
}