param([string] $version, [string] $key)

dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.BusinessLogic.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json

dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json

dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Shared.Configuration.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json

dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.UI.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.UI.Spa.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json
dotnet nuget push ./packages/Toralux.Open.IdentityServer.Admin.UI.Api.$version.nupkg -k $key -s https://api.nuget.org/v3/index.json