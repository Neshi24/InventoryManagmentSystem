FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["tests/AuthService.Tests/AuthService.Tests.csproj", "tests/AuthService.Tests/"]
COPY ["projects/auth-service/AuthService/AuthService.csproj", "projects/auth-service/AuthService/"]
COPY ["packages/CommonPackage/CommonPackage.csproj", "packages/CommonPackage/"]
RUN dotnet restore "tests/AuthService.Tests/AuthService.Tests.csproj"
COPY . .
WORKDIR "/src/tests/AuthService.Tests"
RUN dotnet build "AuthService.Tests.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.Tests.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Tests.dll"]
