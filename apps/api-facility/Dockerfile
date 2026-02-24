FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["apps/api-facility/FacilityApi.csproj", "apps/api-facility/"]
RUN dotnet restore "apps/api-facility/FacilityApi.csproj"
COPY apps/api-facility/ apps/api-facility/
WORKDIR "/src/apps/api-facility"
RUN dotnet build "FacilityApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FacilityApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FacilityApi.dll"]