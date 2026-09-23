// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Open.IdentityServer.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Interfaces;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.DbContexts
{
    public class IdentityServerPersistedGrantDbContext : PersistedGrantDbContext<IdentityServerPersistedGrantDbContext>, IAdminPersistedGrantDbContext
    {
        public IdentityServerPersistedGrantDbContext(DbContextOptions<IdentityServerPersistedGrantDbContext> options)
            : base(options)
        {
        }
    }
}