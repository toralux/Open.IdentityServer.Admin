// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Toralux.Open.IdentityServer.Shared.Configuration.Configuration.Identity;
using ToraluxOpen.IdentityServerAdmin.STS.Identity.Configuration.Interfaces;

namespace ToraluxOpen.IdentityServerAdmin.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {      
        public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
        public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();
    }
}