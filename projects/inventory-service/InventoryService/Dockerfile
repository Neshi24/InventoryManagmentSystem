FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["projects/inventory-service/InventoryService/InventoryService.csproj", "projects/inventory-service/InventoryService/"]
COPY ["shared/Shared/Shared.csproj", "shared/Shared/"]
RUN dotnet restore "projects/inventory-service/InventoryService/InventoryService.csproj"
COPY . .
WORKDIR "/src/projects/inventory-service/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM publish AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
