// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Generic;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.Configuration.Identity;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.Configuration.IdentityServer
{
    public class Client : global::Open.IdentityServer.Models.Client
    {
        public List<Claim> ClientClaims { get; set; } = new List<Claim>();
    }
}
