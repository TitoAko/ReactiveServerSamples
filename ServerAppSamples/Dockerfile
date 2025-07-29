# ---------- build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy rest of source – any .cs edit invalidates this layer
COPY . .
RUN dotnet publish ServerAppSamples/ServerApp.csproj -c Release -o /app

# copy solution & csproj first – cache restore
COPY *.sln .
COPY ServerAppSamples/ServerApp.csproj ServerAppSamples/
RUN dotnet restore

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ServerApp.dll"]
