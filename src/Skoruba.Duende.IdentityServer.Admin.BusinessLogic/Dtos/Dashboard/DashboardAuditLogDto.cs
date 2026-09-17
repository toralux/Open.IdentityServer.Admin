using System;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Dtos.Dashboard;

public class DashboardAuditLogDto
{
    public int Total { get; set; }

    public DateTime Created { get; set; }
}