// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;

namespace Toralux.Open.IdentityServer.STS.Identity.IntegrationTests.Common
{
    public static class RoutesConstants
    {
        public static List<string> GetManageRoutes()
        {
            var manageRoutes = new List<string>
            {
                "Index",
                "ChangePassword",
                "PersonalData",
                "DeletePersonalData",
                "ExternalLogins",
                "Passkeys",
                "TwoFactorAuthentication",
                "ResetAuthenticatorWarning",
                "EnableAuthenticator"
            };

            return manageRoutes;
        }
    }
}
