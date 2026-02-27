# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

# Copy API project
COPY ["Lerevently.Api/*.csproj", "Lerevently.Api/"]

# Copy Common projects
COPY ["Lerevently.Common.Application/*.csproj", "Lerevently.Common.Application/"]
COPY ["Lerevently.Common.Domain/*.csproj", "Lerevently.Common.Domain/"]
COPY ["Lerevently.Common.Infrastructure/*.csproj", "Lerevently.Common.Infrastructure/"]
COPY ["Lerevently.Common.Presentation/*.csproj", "Lerevently.Common.Presentation/"]
COPY ["Lerevently.Modules.Events.Presentation/*.csproj", "Lerevently.Modules.Events.Presentation/"]

# Copy Events Module projects
COPY ["Lerevently.Modules.Attendance.Application/*.csproj", "Lerevently.Modules.Attendance.Application/"]
COPY ["Lerevently.Modules.Attendance.Domain/*.csproj", "Lerevently.Modules.Attendance.Domain/"]
COPY ["Lerevently.Modules.Attendance.Infrastructure/*.csproj", "Lerevently.Modules.Attendance.Infrastructure/"]
COPY ["Lerevently.Modules.Attendance.IntegrationEvents/*.csproj", "Lerevently.Modules.Attendance.IntegrationEvents/"]
COPY ["Lerevently.Modules.Attendance.Presentation/*.csproj", "Lerevently.Modules.Attendance.Presentation/"]

# Copy Events Module projects
COPY ["Lerevently.Modules.Events.Application/*.csproj", "Lerevently.Modules.Events.Application/"]
COPY ["Lerevently.Modules.Events.Domain/*.csproj", "Lerevently.Modules.Events.Domain/"]
COPY ["Lerevently.Modules.Events.Infrastructure/*.csproj", "Lerevently.Modules.Events.Infrastructure/"]
COPY ["Lerevently.Modules.Events.IntegrationEvents/*.csproj", "Lerevently.Modules.Events.IntegrationEvents/"]

# Copy Ticketing Module projects
COPY ["Lerevently.Modules.Ticketing.Application/*.csproj", "Lerevently.Modules.Ticketing.Application/"]
COPY ["Lerevently.Modules.Ticketing.Domain/*.csproj", "Lerevently.Modules.Ticketing.Domain/"]
COPY ["Lerevently.Modules.Ticketing.IntegrationEvents/*.csproj", "Lerevently.Modules.Ticketing.IntegrationEvents/"]
COPY ["Lerevently.Modules.Ticketing.Infrastructure/*.csproj", "Lerevently.Modules.Ticketing.Infrastructure/"]
COPY ["Lerevently.Modules.Ticketing.Presentation/*.csproj", "Lerevently.Modules.Ticketing.Presentation/"]

# Copy Users Module projects
COPY ["Lerevently.Modules.Users.IntegrationEvents/*.csproj", "Lerevently.Modules.Users.IntegrationEvents/"]
COPY ["Lerevently.Modules.Users.Application/*.csproj", "Lerevently.Modules.Users.Application/"]
COPY ["Lerevently.Modules.Users.Domain/*.csproj", "Lerevently.Modules.Users.Domain/"]
COPY ["Lerevently.Modules.Users.Infrastructure/*.csproj", "Lerevently.Modules.Users.Infrastructure/"]
COPY ["Lerevently.Modules.Users.Presentation/*.csproj", "Lerevently.Modules.Users.Presentation/"]


RUN dotnet restore "./Lerevently.Api/Lerevently.Api.csproj"
COPY . .
WORKDIR "/src/Lerevently.Api"
RUN dotnet build "./Lerevently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lerevently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lerevently.Api.dll"]