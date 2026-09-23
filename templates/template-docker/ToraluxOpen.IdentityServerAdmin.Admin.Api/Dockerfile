FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.Api/ToraluxOpen.IdentityServerAdmin.Admin.Api.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.Api/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Shared/ToraluxOpen.IdentityServerAdmin.Shared.csproj", "src/ToraluxOpen.IdentityServerAdmin.Shared/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL/"]
RUN dotnet restore -a $TARGETARCH "src/ToraluxOpen.IdentityServerAdmin.Admin.Api/ToraluxOpen.IdentityServerAdmin.Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/ToraluxOpen.IdentityServerAdmin.Admin.Api"
RUN dotnet build -a $TARGETARCH "ToraluxOpen.IdentityServerAdmin.Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "ToraluxOpen.IdentityServerAdmin.Admin.Api.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "ToraluxOpen.IdentityServerAdmin.Admin.Api.dll"]
