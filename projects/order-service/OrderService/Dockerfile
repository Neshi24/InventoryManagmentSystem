FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["projects/order-service/OrderService/OrderService.csproj", "projects/order-service/OrderService/"]
COPY ["shared/Shared/Shared.csproj", "shared/Shared/"]
RUN dotnet restore "projects/order-service/OrderService/OrderService.csproj"
COPY . .
WORKDIR "/src/projects/order-service/OrderService"
RUN dotnet build "OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM publish AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
