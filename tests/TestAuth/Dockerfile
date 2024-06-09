FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["tests/TestAuth/TestAuth.csproj", "tests/TestAuth/"]
COPY ["projects/auth-service/AuthService/AuthService.csproj", "projects/auth-service/AuthService/"]
COPY ["packages/CommonPackage/CommonPackage.csproj", "packages/CommonPackage/"]
RUN dotnet restore "tests/TestAuth/TestAuth.csproj"
COPY . .
WORKDIR "/src/tests/TestAuth"
RUN dotnet build "TestAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestAuth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestAuth.dll"]
