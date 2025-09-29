# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["api/LostAndFoundAPI/LostAndFoundAPI.csproj", "api/LostAndFoundAPI/"]
RUN dotnet restore "api/LostAndFoundAPI/LostAndFoundAPI.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/api/LostAndFoundAPI"
RUN dotnet build "LostAndFoundAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "LostAndFoundAPI.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LostAndFoundAPI.dll"]
