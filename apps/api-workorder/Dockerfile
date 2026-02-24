FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["apps/api-workorder/WorkOrderApi.csproj", "apps/api-workorder/"]
RUN dotnet restore "apps/api-workorder/WorkOrderApi.csproj"
COPY apps/api-workorder/ apps/api-workorder/
WORKDIR "/src/apps/api-workorder"
RUN dotnet build "WorkOrderApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkOrderApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkOrderApi.dll"]