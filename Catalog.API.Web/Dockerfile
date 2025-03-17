ARG RUNTIMEVERSION=9.0-azurelinux3.0-distroless
ARG SDKVERSION=9.0-azurelinux3.0

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
COPY ["Catalog.API.Application/Catalog.API.Application.csproj", "Catalog.API.Application/Catalog.API.Application.csproj"]
COPY ["Catalog.API.Domain/Catalog.API.Domain.csproj", "Catalog.API.Domain/Catalog.API.Domain.csproj"]
COPY ["Catalog.API.Infrastructure/Catalog.API.Infrastructure.csproj", "Catalog.API.Infrastructure/Catalog.API.Infrastructure.csproj"]
COPY ["Catalog.API.Web/Catalog.API.Web.csproj", "Catalog.API.Web/Catalog.API.Web.csproj"]
COPY ["Services.Common/Services.Common.csproj", "Services.Common/Services.Common.csproj"]

RUN dotnet restore "Catalog.API.Web/Catalog.API.Web.csproj"

COPY . .
WORKDIR /src/Catalog.API.Web
RUN dotnet publish "Catalog.API.Web.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.API.Web.dll"]