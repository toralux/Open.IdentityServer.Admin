FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["src/ToraluxOpen.IdentityServerAdmin.STS.Identity/ToraluxOpen.IdentityServerAdmin.STS.Identity.csproj", "src/ToraluxOpen.IdentityServerAdmin.STS.Identity/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared.csproj", "src/ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared/"]
COPY ["src/ToraluxOpen.IdentityServerAdmin.Shared/ToraluxOpen.IdentityServerAdmin.Shared.csproj", "src/ToraluxOpen.IdentityServerAdmin.Shared/"]
RUN dotnet restore -a $TARGETARCH "src/ToraluxOpen.IdentityServerAdmin.STS.Identity/ToraluxOpen.IdentityServerAdmin.STS.Identity.csproj"
COPY . .
WORKDIR "/src/src/ToraluxOpen.IdentityServerAdmin.STS.Identity"
RUN dotnet build -a $TARGETARCH "ToraluxOpen.IdentityServerAdmin.STS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "ToraluxOpen.IdentityServerAdmin.STS.Identity.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "ToraluxOpen.IdentityServerAdmin.STS.Identity.dll"]