﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["projects/auth-service/AuthService/AuthService.csproj", "projects/auth-service/AuthService/"]
COPY ["packages/CommonPackage/CommonPackage.csproj", "packages/CommonPackage/"]
RUN dotnet restore "projects/auth-service/AuthService/AuthService.csproj"
COPY . .
WORKDIR "/src/projects/auth-service/AuthService"
RUN dotnet build "AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM publish AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]
