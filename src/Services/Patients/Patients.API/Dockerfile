# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Services/Patients/Patients.API/Patients.API.csproj", "src/Services/Patients/Patients.API/"]
COPY ["src/Core/Core.API/Core.API.csproj", "src/Core/Core.API/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Logging/BuildingBlocks.Logging.csproj", "src/BuildingBlocks/BuildingBlocks.Logging/"]
COPY ["src/Core/Core.Infrastructure/Core.Infrastructure.csproj", "src/Core/Core.Infrastructure/"]
COPY ["src/Core/Core.Application/Core.Application.csproj", "src/Core/Core.Application/"]
COPY ["src/Core/Core.Domain/Core.Domain.csproj", "src/Core/Core.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "src/BuildingBlocks/BuildingBlocks.Messaging/"]
COPY ["src/BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/BuildingBlocks/"]
COPY ["src/Services/Patients/Patients.Application/Patients.Application.csproj", "src/Services/Patients/Patients.Application/"]
COPY ["src/Services/Patients/Patients.Domain/Patients.Domain.csproj", "src/Services/Patients/Patients.Domain/"]
COPY ["src/Services/Patients/Patients.Infrastructure/Patients.Infrastructure.csproj", "src/Services/Patients/Patients.Infrastructure/"]
RUN dotnet restore "./src/Services/Patients/Patients.API/Patients.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Patients/Patients.API"
RUN dotnet build "./Patients.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Patients.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Patients.API.dll"]