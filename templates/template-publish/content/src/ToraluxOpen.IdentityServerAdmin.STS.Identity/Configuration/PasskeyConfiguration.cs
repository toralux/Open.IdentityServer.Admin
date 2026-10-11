// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace ToraluxOpen.IdentityServerAdmin.STS.Identity.Configuration
{
    public class PasskeyConfiguration
    {
        public const string SectionName = "PasskeyConfiguration";

        /// <summary>
        /// The relying party domain (e.g. "id.example.com").
        /// Defaults to the host of the current request when empty.
        /// </summary>
        public string ServerDomain { get; set; }

        /// <summary>
        /// Explicit list of allowed origins (e.g. ["https://id.example.com"]).
        /// Required in production when the app is deployed behind a reverse proxy
        /// or when the default origin validation fails.
        /// Leave empty in development — localhost is allowed automatically.
        /// </summary>
        public List<string> AllowedOrigins { get; set; } = new();
    }
}
