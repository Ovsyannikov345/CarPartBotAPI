FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app

USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

USER app
WORKDIR /app

EXPOSE 5127
ENV ASPNETCORE_HTTP_PORTS=5127

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CarPartBotApi/CarPartBotApi.Api/CarPartBotApi.Api.csproj", "CarPartBotApi/CarPartBotApi.Api/"]
RUN dotnet restore "./CarPartBotApi/CarPartBotApi.Api/CarPartBotApi.Api.csproj"

COPY . .

WORKDIR "/src/CarPartBotApi/CarPartBotApi.Api"
RUN dotnet build "./CarPartBotApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CarPartBotApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CarPartBotApi.Api.dll"]