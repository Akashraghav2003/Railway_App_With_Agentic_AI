# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["CaseStudy/RailwayManagmentSystem/RailwayManagmentSystem.csproj", "CaseStudy/RailwayManagmentSystem/"]
COPY ["CaseStudy/BusinessLayer/BusinessLayer.csproj", "CaseStudy/BusinessLayer/"]
COPY ["CaseStudy/ModelLayer/ModelLayer.csproj", "CaseStudy/ModelLayer/"]
COPY ["CaseStudy/RepositoryLayer/RepositoryLayer.csproj", "CaseStudy/RepositoryLayer/"]
COPY ["CaseStudy/RailwayManagmentTesting/RailwayManagmentTesting.csproj", "CaseStudy/RailwayManagmentTesting/"]

RUN dotnet restore "CaseStudy/RailwayManagmentSystem/RailwayManagmentSystem.csproj"

# Copy the entire source code
COPY . .

# Build the application
WORKDIR "/src/CaseStudy/RailwayManagmentSystem"
RUN dotnet build "RailwayManagmentSystem.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "RailwayManagmentSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "RailwayManagmentSystem.dll"]