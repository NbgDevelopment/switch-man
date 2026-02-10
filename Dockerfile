# Use the official .NET 10 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Copy the project file and restore dependencies
COPY NbgDev.SwitchMan.App/*.csproj ./NbgDev.SwitchMan.App/
WORKDIR /source/NbgDev.SwitchMan.App
RUN dotnet restore

# Copy the rest of the application code
WORKDIR /source
COPY NbgDev.SwitchMan.App/. ./NbgDev.SwitchMan.App/

# Build and publish the application
WORKDIR /source/NbgDev.SwitchMan.App
RUN dotnet publish -c Release -o /app --no-restore

# Use the official .NET 10 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./

# Expose port 8080
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "NbgDev.SwitchMan.App.dll"]
