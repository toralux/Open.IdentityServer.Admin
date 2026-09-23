// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.EntityFrameworkCore;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Constants;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Entities;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Interfaces;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminLogDbContext : DbContext, IAdminLogDbContext
    {
        public DbSet<Log> Logs { get; set; }

        public AdminLogDbContext(DbContextOptions<AdminLogDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureLogContext(builder);
        }

        private void ConfigureLogContext(ModelBuilder builder)
        {
            builder.Entity<Log>(log =>
            {
                log.ToTable(TableConsts.Logging);
                log.HasKey(x => x.Id);
                log.Property(x => x.Level).HasMaxLength(128);
            });
        }
    }
}
