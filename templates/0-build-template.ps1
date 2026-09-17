param([string] $packagesVersions)

# This script contains following steps:
# - Download latest version of Toralux.Open.IdentityServer.Admin from git repository
# - Use folders src and tests for project template

$gitProjectFolder = "Toralux.Open.IdentityServer.Admin"
$templateSrc = "template-build/content/src"
$templateRoot = "template-build/content"
$templateTests = "template-build/content/tests"
$templateAdminProject = "template-build/content/src/Toralux.Open.IdentityServer.Admin"

Get-Location

function CleanBinObjFolders { 

    # Clean up after migrations
    dotnet clean $templateAdminProject

    # Clean up bin, obj
    Get-ChildItem .\ -include bin, obj -Recurse | ForEach-Object ($_) { Remove-Item $_.fullname -Force -Recurse }    
}

# Copy the local src and tests folders to the project folder instead of cloning from git
Copy-Item ../src $gitProjectFolder/src -Recurse -Force
Copy-Item ../tests $gitProjectFolder/tests -Recurse -Force

# Copy Docker files and shared resources if they exist
$dockerFiles = @(
    "docker-compose.dcproj",
    ".dockerignore",
    "docker-compose.override.yml",
    "docker-compose.yml",
    "Directory.Build.props"
)

foreach ($file in $dockerFiles) {
    $sourcePath = Join-Path ".." $file
    if (Test-Path $sourcePath) {
        Copy-Item $sourcePath $gitProjectFolder -Force
    }
}

# Copy shared folder if it exists
$sharedSource = Join-Path ".." "shared"
if (Test-Path $sharedSource) {
    Copy-Item $sharedSource $gitProjectFolder -Recurse -Force
}


# Clean up src, tests folders
if ((Test-Path -Path $templateSrc)) { Remove-Item ./$templateSrc -recurse -force }
if ((Test-Path -Path $templateTests)) { Remove-Item ./$templateTests -recurse -force }

# Create src, tests folders
if (!(Test-Path -Path $templateSrc)) { mkdir $templateSrc }
if (!(Test-Path -Path $templateTests)) { mkdir $templateTests }

# Copy the latest src and tests to content
Copy-Item ./$gitProjectFolder/src/* $templateSrc -recurse -force
Copy-Item ./$gitProjectFolder/tests/* $templateTests -recurse -force

# Copy Docker files
Copy-Item ./$gitProjectFolder/docker-compose.dcproj $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/.dockerignore $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/docker-compose.override.yml $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/docker-compose.yml $templateRoot -recurse -force
Copy-Item ./$gitProjectFolder/shared $templateRoot -recurse -force

Copy-Item ./$gitProjectFolder/Directory.Build.props $templateRoot -recurse -force

# Clean up created folders
Remove-Item ./$gitProjectFolder -recurse -force

# Clean solution and folders bin, obj
CleanBinObjFolders

# Remove references

# API
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin.Api/Toralux.Open.IdentityServer.Admin.Api.csproj reference ..\Toralux.Open.IdentityServer.Admin.UI.Api\Toralux.Open.IdentityServer.Admin.UI.Api.csproj

# Admin
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj reference ..\Toralux.Open.IdentityServer.Admin.UI\Toralux.Open.IdentityServer.Admin.UI.csproj
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj reference ..\Toralux.Open.IdentityServer.Shared.Configuration\Toralux.Open.IdentityServer.Shared.Configuration.csproj

# STS
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.STS.Identity/Toralux.Open.IdentityServer.STS.Identity.csproj reference ..\Toralux.Open.IdentityServer.Shared.Configuration\Toralux.Open.IdentityServer.Shared.Configuration.csproj
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.STS.Identity/Toralux.Open.IdentityServer.STS.Identity.csproj reference ..\Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration\Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.csproj

# EF Shared
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj reference ..\Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration\Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.csproj

# Shared
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Shared/Toralux.Open.IdentityServer.Shared.csproj reference ..\Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity\Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.csproj

# EF Shared - remove Admin Storage reference (only Admin Storage is used in template projects)
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj reference ..\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.csproj

# DB specific projects - remove Admin Storage project references
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer.csproj reference ..\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.csproj
dotnet remove ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL.csproj reference ..\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage\Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage.csproj

# Add nuget packages
# Admin
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj package Toralux.Open.IdentityServer.Admin.UI -v $packagesVersions
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj package Toralux.Open.IdentityServer.Admin.UI.Spa -v $packagesVersions
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj package Toralux.Open.IdentityServer.Shared.Configuration -v $packagesVersions

# STS
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.STS.Identity/Toralux.Open.IdentityServer.STS.Identity.csproj package Toralux.Open.IdentityServer.Shared.Configuration -v $packagesVersions
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.STS.Identity/Toralux.Open.IdentityServer.STS.Identity.csproj package Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration -v $packagesVersions

# API
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin.Api/Toralux.Open.IdentityServer.Admin.Api.csproj package Toralux.Open.IdentityServer.Admin.UI.Api -v $packagesVersions

# EF Shared
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj package Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration -v $packagesVersions

# Shared
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Shared/Toralux.Open.IdentityServer.Shared.csproj package Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity -v $packagesVersions

# EF Shared - add Admin Storage package (only Admin Storage is used in template projects)
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj package Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage -v $packagesVersions

# DB specific projects - add Admin Storage NuGet package
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer.csproj package Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage -v $packagesVersions
dotnet add ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL.csproj package Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage -v $packagesVersions

# Clean solution and folders bin, obj
CleanBinObjFolders

# Clean up projects which will be installed via nuget packages
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.BusinessLogic -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Shared.Configuration -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.UI -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.UI.Spa -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.UI.Client -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.UI.Api -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Admin -Force -recurse
Remove-Item ./$templateSrc/Toralux.Open.IdentityServer.Admin.EntityFramework.Admin.Storage -Force -recurse
Remove-Item ./$templateTests -Force -recurse

$csprojPath = "$templateSrc/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj"

# Remove <SpaRoot>, <SpaProxyLaunchCommand>, <SpaProxyServerUrl> from .csproj
if (Test-Path $csprojPath) {
    $csprojContent = Get-Content $csprojPath
    $filteredCsproj = $csprojContent | Where-Object {
        $_ -notmatch '<SpaRoot>.*</SpaRoot>' -and
        $_ -notmatch '<SpaProxyLaunchCommand>.*</SpaProxyLaunchCommand>' -and
        $_ -notmatch '<SpaProxyServerUrl>.*</SpaProxyServerUrl>'
    }
    $filteredCsproj | Set-Content $csprojPath -Encoding UTF8
}

# Remove <ItemGroup> with ProjectReference to UI.Client.esproj from .csproj
if (Test-Path $csprojPath) {
    $csprojContent = Get-Content $csprojPath
    $result = @()
    $inGroup = $false
    $buffer = @()
    foreach ($line in $csprojContent) {
        if ($line -match '<ItemGroup>') {
            $inGroup = $true
            $buffer = @($line)
            continue
        }
        if ($inGroup) {
            $buffer += $line
            if ($line -match '</ItemGroup>') {
                $blockText = $buffer -join "`n"
                if ($blockText -notmatch 'ProjectReference\s+Include\s*=\s*".*Skoruba\.Duende\.IdentityServer\.Admin\.UI\.Client[\\/]+Skoruba\.Duende\.IdentityServer\.Admin\.UI\.Client\.esproj"') {
                    $result += $buffer
                }
                $buffer = @()
                $inGroup = $false
            }
            continue
        }
        $result += $line
    }
    $result | Set-Content $csprojPath -Encoding UTF8
}

# Remove ASPNETCORE_HOSTINGSTARTUPASSEMBLIES from launchSettings.json environmentVariables
$launchSettingsPath = "$templateSrc/Toralux.Open.IdentityServer.Admin/Properties/launchSettings.json"
if (Test-Path $launchSettingsPath) {
    $json = Get-Content $launchSettingsPath -Raw | ConvertFrom-Json
    foreach ($profileName in $json.profiles.PSObject.Properties.Name) {
        $env = $json.profiles.$profileName.environmentVariables
        if ($env) {
            # Remove the specific environment variable if present
            $env.PSObject.Properties.Remove("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES")
        }
    }
    $json | ConvertTo-Json -Depth 20 | Set-Content $launchSettingsPath -Encoding UTF8
}

######################################

# Step 2
$templateNuspecPath = "template-build/Toralux.Open.IdentityServer.Admin.Templates.nuspec"
nuget pack ./$templateNuspecPath -NoDefaultExcludes

######################################
# Step 3
$templateLocalName = "Toralux.Open.IdentityServer.Admin.Templates.$packagesVersions.nupkg"

dotnet new --uninstall Toralux.Open.IdentityServer.Admin.Templates
dotnet new -i ./$templateLocalName

######################################
# Step 4
# Create template for fixing project name
dotnet new toralux.open-isadmin --name ToraluxOpen.IdentityServerAdmin --title "Toralux Open IdentityServer Admin" --adminrole ToraluxIdentityAdminAdministrator --adminclientid toralux_identity_admin_v3 --adminclientsecret toralux_admin_client_secret --requirepushedauthorization false

######################################
# Step 5
# Replace files

CleanBinObjFolders

$templateFiles = Get-ChildItem ./ToraluxOpen.IdentityServerAdmin/src -include *.cs, *.csproj, *.cshtml -Recurse
foreach ($file in $templateFiles) {
    Write-Host $file.PSPath

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "ToraluxOpen.IdentityServerAdmin.Shared.Configuration", "Toralux.Open.IdentityServer.Shared.Configuration" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "ToraluxOpen.IdentityServerAdmin.Admin.UI", "Toralux.Open.IdentityServer.Admin.UI" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "ToraluxOpen.IdentityServerAdmin.Admin.UI.Api", "Toralux.Open.IdentityServer.Admin.UI.Api" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "ToraluxOpen.IdentityServerAdmin.Admin.BusinessLogic", "Toralux.Open.IdentityServer.Admin.BusinessLogic" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework", "Toralux.Open.IdentityServer.Admin.EntityFramework" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Toralux.Open.IdentityServer.Admin.EntityFramework.Shared", "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL", "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline

    (Get-Content $file.PSPath -raw -Encoding UTF8) |
    Foreach-Object { $_ -replace "Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer", "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer" } |
    Out-File $file.PSPath -Encoding UTF8 -NoNewline
}


CleanBinObjFolders
