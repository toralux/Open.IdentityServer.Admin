using System.Threading;
using System.Threading.Tasks;
using Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.DashboardIdentity;

namespace Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;

public interface IDashboardIdentityService
{
    public Task<DashboardIdentityDto> GetIdentityDashboardAsync(CancellationToken cancellationToken = default);
}