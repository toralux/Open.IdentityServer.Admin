// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Globalization;
using Microsoft.AspNetCore.Identity;

namespace Skoruba.Open.IdentityServer.Admin.EntityFramework.Identity.Resources
{
    internal static class IdentityRepositoryErrors
    {
        internal static IdentityError UserDoesNotExist(object userId)
        {
            return CreateError(
                nameof(UserDoesNotExist),
                string.Format(
                    CultureInfo.InvariantCulture,
                    IdentityRepositoryResource.UserDoesNotExist,
                    Convert.ToString(userId, CultureInfo.InvariantCulture)));
        }

        internal static IdentityError UserClaimDoesNotExist(int claimId)
        {
            return CreateError(
                nameof(UserClaimDoesNotExist),
                string.Format(
                    CultureInfo.InvariantCulture,
                    IdentityRepositoryResource.UserClaimDoesNotExist,
                    claimId));
        }

        internal static IdentityError RoleDoesNotExist(object roleId)
        {
            return CreateError(
                nameof(RoleDoesNotExist),
                string.Format(
                    CultureInfo.InvariantCulture,
                    IdentityRepositoryResource.RoleDoesNotExist,
                    Convert.ToString(roleId, CultureInfo.InvariantCulture)));
        }

        internal static IdentityError RoleClaimDoesNotExist(int claimId)
        {
            return CreateError(
                nameof(RoleClaimDoesNotExist),
                string.Format(
                    CultureInfo.InvariantCulture,
                    IdentityRepositoryResource.RoleClaimDoesNotExist,
                    claimId));
        }

        private static IdentityError CreateError(string code, string description)
        {
            return new IdentityError
            {
                Code = code,
                Description = description
            };
        }
    }
}
