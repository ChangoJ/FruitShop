FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiamos todos los .csproj primero para que Docker cachee la capa de restore.
# Si solo cambia código fuente (no dependencias), esta capa no se reconstruye.
COPY ["FruitShop.Api/FruitShop.Api.csproj", "FruitShop.Api/"]
COPY ["FruitShop.Application/FruitShop.Application.csproj", "FruitShop.Application/"]
COPY ["FruitShop.Domain/FruitShop.Domain.csproj", "FruitShop.Domain/"]
COPY ["FruitShop.Infrastructure/FruitShop.Infrastructure.csproj", "FruitShop.Infrastructure/"]
RUN dotnet restore "./FruitShop.Api/FruitShop.Api.csproj"

COPY . .
WORKDIR "/src/FruitShop.Api"
RUN dotnet build "./FruitShop.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FruitShop.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FruitShop.Api.dll"]
