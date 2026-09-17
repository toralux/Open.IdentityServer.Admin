using System.Threading;
using System.Threading.Tasks;
using Skoruba.Open.IdentityServer.Admin.EntityFramework.Entities;

namespace Skoruba.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardDataView> GetDashboardIdentityServerAsync(int auditLogsLastNumberOfDays,
        CancellationToken cancellationToken = default);
}