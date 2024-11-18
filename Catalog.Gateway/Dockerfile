ARG VERSION=9.0-alpine

FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS="8080"
ENV ASPNETCORE_ENVIRONMENT="Development"
ENV DOTNET_RUNNING_IN_CONTAINER="true"
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="0"
EXPOSE 8080/tcp

FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build
WORKDIR /src

COPY "Directory.Packages.props" "Directory.Packages.props"
COPY "Directory.Build.props" "Directory.Build.props"
COPY "NuGet.config" "NuGet.config"
COPY ["Catalog.Gateway/Catalog.Gateway.csproj", "Catalog.Gateway/Catalog.Gateway.csproj"]

RUN dotnet restore "Catalog.Gateway/Catalog.Gateway.csproj"

COPY . .
WORKDIR /src/Catalog.Gateway
RUN dotnet publish "Catalog.Gateway.csproj" --no-restore -c Release -o /app/publish /p:UseAppHost=false

FROM build AS publish

FROM base AS final
USER $APP_UID
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Gateway.dll"]