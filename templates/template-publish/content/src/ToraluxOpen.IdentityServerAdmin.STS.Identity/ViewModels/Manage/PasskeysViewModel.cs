// Copyright (c) Duende Software. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// Modified by Jan Skoruba

using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ToraluxOpen.IdentityServerAdmin.STS.Identity.ViewModels.Account;

namespace ToraluxOpen.IdentityServerAdmin.STS.Identity.ViewModels.Manage
{
    public class PasskeysViewModel
    {
        public IList<UserPasskeyInfo> CurrentPasskeys { get; set; }
        public string StatusMessage { get; set; }
    }

    public class PasskeysInputModel
    {
        public string CredentialId { get; set; }
        public string Action { get; set; }
        public PasskeyInputModel Passkey { get; set; }
    }

    public class RenamePasskeyViewModel
    {
        public string CredentialId { get; set; }
        public string Name { get; set; }
        public string StatusMessage { get; set; }
    }
}
