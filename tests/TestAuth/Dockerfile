FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["tests/TestAuth/TestAuth.csproj", "tests/TestAuth/"]
RUN dotnet restore "tests/TestAuth/TestAuth.csproj"

COPY . .
WORKDIR "/src/tests/TestAuth"

FROM build AS publish
RUN dotnet publish "TestAuth.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "test" "TestAuth.dll"]

