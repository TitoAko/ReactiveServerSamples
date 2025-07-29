FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish ClientAppSamples/ClientApp.csproj -c Release -o /app

COPY *.sln .
COPY ClientAppSamples/ClientApp.csproj ClientAppSamples/
RUN dotnet restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ClientApp.dll"]
