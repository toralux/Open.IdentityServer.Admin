using Skoruba.Open.IdentityServer.Admin.UI.Services.Configurations;

namespace Skoruba.Open.IdentityServer.Admin.UI.Services;

public class SkorubaAdminUIOptions
{
    public AdminConfiguration AdminConfiguration { get; set; } = new();
}
