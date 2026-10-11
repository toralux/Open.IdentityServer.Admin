$templateSrc = "template-publish/content/src"
$templateContent = "template-publish/content"
$temporaryProjectFolder = "ToraluxOpen.IdentityServerAdmin"
$templateDockerFolder = "template-docker"

# Remove original src folder for publish folder
if ((Test-Path -Path $templateSrc)) { Remove-Item ./$templateSrc -recurse -force }

# Copy new src folder
Copy-Item ./$temporaryProjectFolder/src ./$templateSrc -recurse -force
Copy-Item ./$temporaryProjectFolder/docker-compose.yml ./$templateContent/docker-compose.yml -recurse -force

# Copy docker files for Admin, Api and STS
Copy-Item ./$templateDockerFolder/ToraluxOpen.IdentityServerAdmin.Admin/* $templateSrc/ToraluxOpen.IdentityServerAdmin.Admin -recurse -force
Copy-Item ./$templateDockerFolder/ToraluxOpen.IdentityServerAdmin.Admin.Api/* $templateSrc/ToraluxOpen.IdentityServerAdmin.Admin.Api -recurse -force
Copy-Item ./$templateDockerFolder/ToraluxOpen.IdentityServerAdmin.STS.Identity/* $templateSrc/ToraluxOpen.IdentityServerAdmin.STS.Identity -recurse -force