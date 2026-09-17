using System.Threading;
using System.Threading.Tasks;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Dtos.Dashboard;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardIdentityServerAsync(int auditLogsLastNumberOfDays,
        CancellationToken cancellationToken = default);
}