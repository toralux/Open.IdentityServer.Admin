FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY ["src/Toralux.Open.IdentityServer.Admin.Api/Toralux.Open.IdentityServer.Admin.Api.csproj", "src/Toralux.Open.IdentityServer.Admin.Api/"]
COPY ["src/Toralux.Open.IdentityServer.Shared.Configuration/Toralux.Open.IdentityServer.Shared.Configuration.csproj", "src/Toralux.Open.IdentityServer.Shared.Configuration/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Shared/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Configuration/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Identity/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework/Toralux.Open.IdentityServer.Admin.EntityFramework.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.Extensions/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared/Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared.csproj", "src/Toralux.Open.IdentityServer.Admin.BusinessLogic.Shared/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity/Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity.csproj", "src/Toralux.Open.IdentityServer.Admin.BusinessLogic.Identity/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.BusinessLogic/Toralux.Open.IdentityServer.Admin.BusinessLogic.csproj", "src/Toralux.Open.IdentityServer.Admin.BusinessLogic/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer.csproj", "src/Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer/"]
COPY ["src/Toralux.Open.IdentityServer.Admin.UI.Api/Toralux.Open.IdentityServer.Admin.UI.Api.csproj", "src/Toralux.Open.IdentityServer.Admin.UI.Api/"]
COPY ["src/Toralux.Open.IdentityServer.Shared/Toralux.Open.IdentityServer.Shared.csproj", "src/Toralux.Open.IdentityServer.Shared/"]

RUN dotnet restore -a $TARGETARCH "src/Toralux.Open.IdentityServer.Admin.Api/Toralux.Open.IdentityServer.Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/Toralux.Open.IdentityServer.Admin.Api"
RUN dotnet build -a $TARGETARCH "Toralux.Open.IdentityServer.Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "Toralux.Open.IdentityServer.Admin.Api.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Toralux.Open.IdentityServer.Admin.Api.dll"]
