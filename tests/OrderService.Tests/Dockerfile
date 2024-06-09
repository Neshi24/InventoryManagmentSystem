FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["tests/OrderService.Tests/OrderService.Tests.csproj", "tests/OrderService.Tests/"]
RUN dotnet restore "tests/OrderService.Tests/OrderService.Tests.csproj"
COPY . .
WORKDIR "/src/tests/OrderService.Tests"
RUN dotnet build "OrderService.Tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.Tests.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Tests.dll"]
