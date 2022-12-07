ARG VERSION=7.0-alpine

FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build
WORKDIR /src
COPY ["Catalog.Auth/Catalog.Auth.csproj", "Catalog.Auth/"]
RUN dotnet restore "Catalog.Auth/Catalog.Auth.csproj"
COPY . .
WORKDIR "/src/Catalog.Auth"
RUN dotnet build "Catalog.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Catalog.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.Auth.dll"]