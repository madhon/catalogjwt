ARG RUNTIMEVERSION=10.0-noble-chiseled-extra
ARG SDKVERSION=10.0-noble

FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS="8080"
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV DOTNET_RUNNING_IN_CONTAINER="true"
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="0"
EXPOSE 8080/tcp

FROM mcr.microsoft.com/dotnet/sdk:$SDKVERSION AS build
WORKDIR /src

COPY "Directory.Packages.props" "Directory.Packages.props"
COPY "Directory.Build.props" "Directory.Build.props"
COPY "NuGet.config" "NuGet.config"
COPY ["Services.Common/Services.Common.csproj", "Services.Common/Services.Common.csproj"]
COPY ["Catalog.Auth/Catalog.Auth.csproj", "Catalog.Auth/Catalog.Auth.csproj"]

RUN dotnet restore "Catalog.Auth/Catalog.Auth.csproj"

COPY . .
WORKDIR /src/Catalog.Auth
RUN dotnet publish "Catalog.Auth.csproj" -c Release -o /app/publish

FROM build AS publish

FROM base AS final
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Auth.dll"]