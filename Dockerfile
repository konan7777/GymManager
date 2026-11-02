
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GymManager.Api/GymManager.Api.csproj", "GymManager.Api/"]

RUN dotnet restore "GymManager.Api/GymManager.Api.csproj"

COPY . .

WORKDIR "/src/GymManager.Api"

RUN dotnet publish "GymManager.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "GymManager.Api.dll"]
