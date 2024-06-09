# Use the official ASP.NET Core image as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the SDK image to build the application
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

# Use the ASP.NET Core runtime image for the final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "test", "AuthService.Tests.dll"]
