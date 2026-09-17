$packagesOutput = ".\packages"

# Clean packages output directory
if (Test-Path $packagesOutput) {
    Get-ChildItem -Path $packagesOutput -Force | Remove-Item -Recurse -Force
}

# Build SPA assets for client before packing
$clientPath = ".\..\src\Toralux.Open.IdentityServer.Admin.UI.Client"
if (Test-Path $clientPath) {
    Push-Location $clientPath
    npm run build:spa
    if ($LASTEXITCODE -ne 0) {
        Pop-Location
        throw "Client build failed (npm run build:spa)."
    }
    Pop-Location
}
else {
    throw "Client path not found: $clientPath"
}

# Business Logic
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.BusinessLogic\Toralux.Open.IdentityServer.Admin.BusinessLogic.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity\Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared\Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Shared.Configuration\Toralux.Open.IdentityServer.Shared.Configuration.csproj -c Release -o $packagesOutput

# EF
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework\Toralux.Open.IdentityServer.Admin.EntityFramework.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions\Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework.Identity\Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework.Shared\Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration\Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.csproj -c Release -o $packagesOutput

# UI
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.UI\Toralux.Open.IdentityServer.Admin.UI.csproj -c Release -o $packagesOutput
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.UI.Spa\Toralux.Open.IdentityServer.Admin.UI.Spa.csproj -c Release -o $packagesOutput

# API
dotnet pack .\..\src\Toralux.Open.IdentityServer.Admin.UI.Api\Toralux.Open.IdentityServer.Admin.UI.Api.csproj -c Release -o $packagesOutput
