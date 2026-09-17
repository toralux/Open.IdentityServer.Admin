// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Mappers.Customization
{
    public interface IIdentityRoleMappingCustomizer<TRoleDto, TRole>
    {
        void MapDtoToEntity(TRoleDto source, TRole destination);
        void MapEntityToDto(TRole source, TRoleDto destination);
    }
}
