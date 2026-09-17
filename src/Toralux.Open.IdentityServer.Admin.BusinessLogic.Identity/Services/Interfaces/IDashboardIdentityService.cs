using System.Threading;
using System.Threading.Tasks;
using Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Dtos.DashboardIdentity;

namespace Skoruba.Open.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;

public interface IDashboardIdentityService
{
    public Task<DashboardIdentityDto> GetIdentityDashboardAsync(CancellationToken cancellationToken = default);
}