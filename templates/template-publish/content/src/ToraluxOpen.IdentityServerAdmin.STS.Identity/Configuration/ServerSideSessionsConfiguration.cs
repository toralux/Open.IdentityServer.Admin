// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

namespace ToraluxOpen.IdentityServerAdmin.STS.Identity.Configuration
{
    public class ServerSideSessionsConfiguration
    {
        public const string SectionName = nameof(ServerSideSessionsConfiguration);

        public bool Enabled { get; set; } = false;
    }
}