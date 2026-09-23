using System;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Entities;

public class DashboardAuditLogDataView
{
    public int Total { get; set; }

    public DateTime Created { get; set; }
}