FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["tests/InventoryService.Tests/InventoryService.Tests.csproj", "tests/InventoryService.Tests/"]
RUN dotnet restore "tests/InventoryService.Tests/InventoryService.Tests.csproj"
COPY . .
WORKDIR "/src/tests/InventoryService.Tests"
RUN dotnet build "InventoryService.Tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.Tests.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.Tests.dll"]
