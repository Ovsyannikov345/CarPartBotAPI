FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app

EXPOSE 5127
ENV ASPNETCORE_HTTP_PORTS=5127

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["CarPartBotApi/CarPartBotApi.Api.csproj", "CarPartBotApi/"]
RUN dotnet restore "./CarPartBotApi/CarPartBotApi.csproj"

# Copy the rest of the source code and build
COPY . .
WORKDIR "/src/CarPartBotApi"
RUN dotnet build "./CarPartBotApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CarPartBotApi.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Copy the published app to the base runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CarPartBotApi.Api.dll"]
