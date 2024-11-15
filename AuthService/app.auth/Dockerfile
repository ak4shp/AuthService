# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081



# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet tool install --global dotnet-ef --version 8.*

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./app.auth.csproj", "./"]
RUN dotnet restore "./app.auth.csproj"

COPY . .
WORKDIR "/src/"
RUN dotnet build "app.auth.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN dotnet-ef database drop --force
RUN dotnet-ef migrations remove
RUN dotnet-ef migrations ADD InitialCreate
RUN dotnet-ef database update

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./app.auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "app.auth.dll"]