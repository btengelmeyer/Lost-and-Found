#!/bin/bash

# Navigate to the API directory
cd api/LostAndFoundAPI

# Restore dependencies
dotnet restore

# Build the application
dotnet build --configuration Release

# Run the application
dotnet run --configuration Release --urls="http://0.0.0.0:$PORT"
