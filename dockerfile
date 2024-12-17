FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["GameOfLife.API/GameOfLife.API.csproj", "GameOfLife.API/"]
COPY ["GameOfLife.Core/GameOfLife.Core.csproj", "GameOfLife.Core/"]
COPY ["GameOfLife.Infrastructure/GameOfLife.Infrastructure.csproj", "GameOfLife.Infrastructure/"]
RUN dotnet restore "GameOfLife.API/GameOfLife.API.csproj"

# Copy the rest of the code
COPY . .

# Build the application
RUN dotnet build "GameOfLife.API/GameOfLife.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "GameOfLife.API/GameOfLife.API.csproj" -c Release -o /app/publish

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameOfLife.API.dll"]
