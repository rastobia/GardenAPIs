FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["src/GardenApi.API/GardenApi.API.csproj", "src/GardenApi.API/"]
COPY ["src/GardenApi.Application/GardenApi.Application.csproj", "src/GardenApi.Application/"]
COPY ["src/GardenApi.Infrastructure/GardenApi.Infrastructure.csproj", "src/GardenApi.Infrastructure/"]
COPY ["src/GardenApi.Domain/GardenApi.Domain.csproj", "src/GardenApi.Domain/"]
RUN dotnet restore "src/GardenApi.API/GardenApi.API.csproj"

COPY . .
WORKDIR "/src/src/GardenApi.API"
RUN dotnet publish "GardenApi.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GardenApi.API.dll"]
