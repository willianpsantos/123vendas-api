#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["123Vendas.Vendas.API/123Vendas.Vendas.API.csproj", "123Vendas.Vendas.API/"]
RUN dotnet restore "./123Vendas.Vendas.API/123Vendas.Vendas.API.csproj"
COPY . .
WORKDIR "/src/123Vendas.Vendas.API"
RUN dotnet build "./123Vendas.Vendas.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./123Vendas.Vendas.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "123Vendas.Vendas.API.dll"]