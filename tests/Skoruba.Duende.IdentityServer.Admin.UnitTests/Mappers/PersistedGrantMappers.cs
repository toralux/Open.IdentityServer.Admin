// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using Bogus;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentAssertions;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Mappers;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Entities;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.Common;
using Toralux.Open.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Toralux.Open.IdentityServer.Admin.UnitTests.Mappers
{
    public class PersistedGrantMappers
    {
        private static readonly Faker Faker = new();

        [Fact]
        public void CanMapPersistedGrantToModel()
        {
            var persistedGrantKey = Guid.NewGuid().ToString();

            //Generate entity
            var persistedGrant = PersistedGrantMock.GenerateRandomPersistedGrant(persistedGrantKey);

            //Try map to DTO
            var persistedGrantDto = persistedGrant.ToModel();

            //Asert
            persistedGrantDto.Should().NotBeNull();

            persistedGrantDto.Should().BeEquivalentTo(persistedGrant);
        }

        [Fact]
        public void CanMapPagedPersistedGrantDataViewToModel()
        {
            var grants = new PagedList<PersistedGrantDataView>
            {
                TotalCount = 10,
                PageSize = 5
            };
            grants.Data.Add(new PersistedGrantDataView
            {
                SubjectId = Faker.Random.Guid().ToString(),
                SubjectName = Faker.Name.FullName()
            });
            grants.Data.Add(new PersistedGrantDataView
            {
                SubjectId = Faker.Random.Guid().ToString(),
                SubjectName = Faker.Name.FullName()
            });

            var grantsDto = grants.ToModel();

            grantsDto.Should().NotBeNull();
            grantsDto.TotalCount.Should().Be(grants.TotalCount);
            grantsDto.PageSize.Should().Be(grants.PageSize);
            grantsDto.PersistedGrants.Should().HaveCount(grants.Data.Count);
            grantsDto.PersistedGrants.Select(x => x.SubjectId).Should().BeEquivalentTo(grants.Data.Select(x => x.SubjectId));
        }

        [Fact]
        public void CanMapPagedPersistedGrantsToModel()
        {
            var grants = new PagedList<PersistedGrant>
            {
                TotalCount = 8,
                PageSize = 4
            };
            grants.Data.Add(PersistedGrantMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString()));
            grants.Data.Add(PersistedGrantMock.GenerateRandomPersistedGrant(Guid.NewGuid().ToString()));

            var grantsDto = grants.ToModel();

            grantsDto.Should().NotBeNull();
            grantsDto.TotalCount.Should().Be(grants.TotalCount);
            grantsDto.PageSize.Should().Be(grants.PageSize);
            grantsDto.PersistedGrants.Should().HaveCount(grants.Data.Count);
            grantsDto.PersistedGrants.Select(x => x.Key).Should().BeEquivalentTo(grants.Data.Select(x => x.Key));
        }
    }
}