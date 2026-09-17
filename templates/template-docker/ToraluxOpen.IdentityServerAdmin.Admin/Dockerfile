FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:22-bookworm-slim AS node

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    NUGET_XMLDOC_MODE=skip
WORKDIR /src

COPY --from=node /usr/local/bin/node /usr/local/bin/node
COPY --from=node /usr/local/lib/node_modules /usr/local/lib/node_modules
RUN ln -s /usr/local/lib/node_modules/npm/bin/npm-cli.js /usr/local/bin/npm \
    && npm install -g npm@latest \
    && node --version && npm --version

COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin/ToraluxOpen.IdentityServerAdmin.Admin.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Identity/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Identity.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Identity/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Extensions/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Extensions.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Extensions/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Configuration/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Configuration.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Configuration/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.UI/ToraluxOpen.IdentityServerAdmin.Admin.UI.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.UI/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Shared.Configuration/ToraluxOpen.IdentityServerAdmin.Shared.Configuration.csproj", "src/ToraluxOpen.IdentityServerAdmin.Shared.Configuration/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared/"]

RUN dotnet restore -a $TARGETARCH "src/ToraluxOpen.IdentityServerAdmin.Admin/ToraluxOpen.IdentityServerAdmin.Admin.csproj"

COPY . .
WORKDIR "/src/src/ToraluxOpen.IdentityServerAdmin.Admin"
RUN dotnet build -a $TARGETARCH "ToraluxOpen.IdentityServerAdmin.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "ToraluxOpen.IdentityServerAdmin.Admin.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "ToraluxOpen.IdentityServerAdmin.Admin.dll"]
