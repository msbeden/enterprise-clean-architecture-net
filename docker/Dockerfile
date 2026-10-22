# Multi-stage Dockerfile for Enterprise Clean Architecture .NET Web API

# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8080

# Stage 2: Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file definitions for optimal dependency caching
COPY ["src/Core/Enterprise.Domain/Enterprise.Domain.csproj", "src/Core/Enterprise.Domain/"]
COPY ["src/Core/Enterprise.Application/Enterprise.Application.csproj", "src/Core/Enterprise.Application/"]
COPY ["src/Infrastructure/Enterprise.Persistence/Enterprise.Persistence.csproj", "src/Infrastructure/Enterprise.Persistence/"]
COPY ["src/Infrastructure/Enterprise.Infrastructure/Enterprise.Infrastructure.csproj", "src/Infrastructure/Enterprise.Infrastructure/"]
COPY ["src/Presentation/Enterprise.WebAPI/Enterprise.WebAPI.csproj", "src/Presentation/Enterprise.WebAPI/"]

RUN dotnet restore "src/Presentation/Enterprise.WebAPI/Enterprise.WebAPI.csproj"

# Copy remaining source code and build
COPY . .
WORKDIR "/src/src/Presentation/Enterprise.WebAPI"
RUN dotnet build "Enterprise.WebAPI.csproj" -c Release -o /app/build

# Stage 3: Publish stage
FROM build AS publish
RUN dotnet publish "Enterprise.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Enterprise.WebAPI.dll"]
