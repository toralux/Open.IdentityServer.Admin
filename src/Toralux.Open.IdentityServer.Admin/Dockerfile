FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM node:22-bookworm-slim AS node

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

COPY ["src/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj", "src/Toralux.Open.IdentityServer.Admin/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework/Toralux.Open.IdentityServer.Admin.EntityFramework.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.UI/Toralux.Open.IdentityServer.Admin.UI.csproj", "src/Toralux.Open.IdentityServer.Admin.UI/"]
COPY ["src/Toralux.Open.IdentityServer.Shared.Configuration/Toralux.Open.IdentityServer.Shared.Configuration.csproj", "src/Toralux.Open.IdentityServer.Shared.Configuration/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/"]

RUN dotnet restore -a $TARGETARCH "src/Toralux.Open.IdentityServer.Admin/Toralux.Open.IdentityServer.Admin.csproj"

COPY . .
WORKDIR "/src/src/Toralux.Open.IdentityServer.Admin"
RUN dotnet build -a $TARGETARCH "Toralux.Open.IdentityServer.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "Toralux.Open.IdentityServer.Admin.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Toralux.Open.IdentityServer.Admin.dll"]
