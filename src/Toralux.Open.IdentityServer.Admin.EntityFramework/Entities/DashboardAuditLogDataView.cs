using System;

namespace Skoruba.Open.IdentityServer.Admin.EntityFramework.Entities;

public class DashboardAuditLogDataView
{
    public int Total { get; set; }

    public DateTime Created { get; set; }
}