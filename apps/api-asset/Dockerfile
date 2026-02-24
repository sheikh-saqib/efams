FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["apps/api-asset/AssetApi.csproj", "apps/api-asset/"]
RUN dotnet restore "apps/api-asset/AssetApi.csproj"
COPY apps/api-asset/ apps/api-asset/
WORKDIR "/src/apps/api-asset"
RUN dotnet build "AssetApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AssetApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AssetApi.dll"]