# -------- build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ServerApp/ServerApp.csproj -c Release -o /app

# -------- runtime stage --------
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ServerApp.dll"]
