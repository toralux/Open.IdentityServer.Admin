using System.Threading;
using System.Threading.Tasks;
using Toralux.Open.IdentityServer.Admin.EntityFramework.Entities;

namespace Toralux.Open.IdentityServer.Admin.EntityFramework.Repositories.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardDataView> GetDashboardIdentityServerAsync(int auditLogsLastNumberOfDays,
        CancellationToken cancellationToken = default);
}