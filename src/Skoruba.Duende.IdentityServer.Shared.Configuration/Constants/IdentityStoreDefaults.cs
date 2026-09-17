using System;
using Microsoft.AspNetCore.Identity;

namespace Skoruba.Open.IdentityServer.Shared.Configuration.Constants
{
    public static class IdentityStoreDefaults
    {
        public const int MaxLengthForKeys = 450;
        public static readonly Version SchemaVersion = IdentitySchemaVersions.Version3;
    }
}
