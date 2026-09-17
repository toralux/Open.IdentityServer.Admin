// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Riok.Mapperly.Abstractions;
using Skoruba.Open.IdentityServer.Admin.UI.Api.Dtos.PersistedGrants;
using ConfigPersistedGrantDto = Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant.PersistedGrantDto;
using ConfigPersistedGrantsDto = Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Grant.PersistedGrantsDto;
using IdentityPersistedGrantDto = Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Grant.PersistedGrantDto;
using IdentityPersistedGrantsDto = Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Grant.PersistedGrantsDto;

namespace Skoruba.Open.IdentityServer.Admin.UI.Api.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
    internal static partial class PersistedGrantApiMapper
    {
        public static partial PersistedGrantApiDto ToPersistedGrantApiDto(ConfigPersistedGrantDto source);
        public static partial PersistedGrantSubjectApiDto ToPersistedGrantSubjectApiDto(ConfigPersistedGrantDto source);
        public static partial PersistedGrantSubjectsApiDto ToPersistedGrantSubjectsApiDto(ConfigPersistedGrantsDto source);
        public static partial PersistedGrantsApiDto ToPersistedGrantsApiDto(ConfigPersistedGrantsDto source);

        public static partial PersistedGrantApiDto ToPersistedGrantApiDto(IdentityPersistedGrantDto source);
        public static partial PersistedGrantSubjectApiDto ToPersistedGrantSubjectApiDto(IdentityPersistedGrantDto source);
        public static partial PersistedGrantSubjectsApiDto ToPersistedGrantSubjectsApiDto(IdentityPersistedGrantsDto source);
        public static partial PersistedGrantsApiDto ToPersistedGrantsApiDto(IdentityPersistedGrantsDto source);
    }

    public static class PersistedGrantApiMappers
    {
        public static PersistedGrantApiDto ToPersistedGrantApiDto(this ConfigPersistedGrantDto source) => PersistedGrantApiMapper.ToPersistedGrantApiDto(source);
        public static PersistedGrantApiDto ToPersistedGrantApiDto(this IdentityPersistedGrantDto source) => PersistedGrantApiMapper.ToPersistedGrantApiDto(source);

        public static PersistedGrantSubjectApiDto ToPersistedGrantSubjectApiDto(this ConfigPersistedGrantDto source) => PersistedGrantApiMapper.ToPersistedGrantSubjectApiDto(source);
        public static PersistedGrantSubjectApiDto ToPersistedGrantSubjectApiDto(this IdentityPersistedGrantDto source) => PersistedGrantApiMapper.ToPersistedGrantSubjectApiDto(source);

        public static PersistedGrantSubjectsApiDto ToPersistedGrantSubjectsApiDto(this ConfigPersistedGrantsDto source) => PersistedGrantApiMapper.ToPersistedGrantSubjectsApiDto(source);
        public static PersistedGrantSubjectsApiDto ToPersistedGrantSubjectsApiDto(this IdentityPersistedGrantsDto source) => PersistedGrantApiMapper.ToPersistedGrantSubjectsApiDto(source);

        public static PersistedGrantsApiDto ToPersistedGrantsApiDto(this ConfigPersistedGrantsDto source) => PersistedGrantApiMapper.ToPersistedGrantsApiDto(source);
        public static PersistedGrantsApiDto ToPersistedGrantsApiDto(this IdentityPersistedGrantsDto source) => PersistedGrantApiMapper.ToPersistedGrantsApiDto(source);
    }
}
