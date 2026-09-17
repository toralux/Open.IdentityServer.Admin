using System;
using System.Reflection;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.Configuration;
using SqlMigrationAssembly = Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer.Helpers.MigrationAssembly;
using PostgreSQLMigrationAssembly = Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL.Helpers.MigrationAssembly;

namespace Toralux.Open.IdentityServer.Admin.Api.Configuration;

public static class MigrationAssemblyConfiguration
{
    public static string GetMigrationAssemblyByProvider(DatabaseProviderConfiguration databaseProvider)
    {
        return databaseProvider.ProviderType switch
        {
            DatabaseProviderType.SqlServer => typeof(SqlMigrationAssembly).GetTypeInfo().Assembly.GetName().Name,
            DatabaseProviderType.PostgreSQL => typeof(PostgreSQLMigrationAssembly).GetTypeInfo()
                .Assembly.GetName()
                .Name,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
