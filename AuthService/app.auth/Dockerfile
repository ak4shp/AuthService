# Use the .NET SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app /p:UseAppHost=false

# Use the .NET runtime to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .
COPY AuthService.db /app/AuthService.db
EXPOSE 8080
ENTRYPOINT ["dotnet", "app.auth.dll"]
