# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Copy the project files and restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["app.auth/app.auth.csproj", "app.auth/"]
RUN dotnet restore "app.auth/app.auth.csproj"

COPY . .
WORKDIR "/src/app.auth"
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY app.auth/AuthService.db ./AuthService.db
COPY app.auth/appsettings.json ./appsettings.json
ENTRYPOINT ["dotnet", "app.auth.dll"]







## Use the .NET SDK to build the application
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#WORKDIR /src
#COPY . .
#RUN dotnet publish -c Release -o /app /p:UseAppHost=false
#
## Use the .NET runtime to run the application
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app
#COPY --from=build /app .
#COPY AuthService.db /app/AuthService.db
#EXPOSE 8080
#ENTRYPOINT ["dotnet", "app.auth.dll"]





## See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
## This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#
#
## This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#
#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet tool install --global dotnet-ef --version 8.*
#
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["./app.auth.csproj", "./"]
#RUN dotnet restore "./app.auth.csproj"
#
#COPY . .
#WORKDIR "/src/"
#RUN dotnet build "app.auth.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#RUN dotnet-ef database drop --force
#RUN dotnet-ef migrations remove
#RUN dotnet-ef migrations ADD InitialCreate
#RUN dotnet-ef database update
#
## This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./app.auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
## This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "app.auth.dll"]