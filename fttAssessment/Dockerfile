# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files
COPY ["fttAssessment.csproj", "./"]
RUN dotnet restore "fttAssessment.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "fttAssessment.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "fttAssessment.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Create logs directory and give permissions
RUN mkdir -p /app/Logs && \
    chmod 777 /app/Logs

# Copy published content
COPY --from=publish /app/publish .

# Ensure log4net config is accessible
COPY ["log4net.config", "./"]
COPY ["appsettings.json", "./"]

# Expose port
EXPOSE 80
EXPOSE 443

# Set entry point
ENTRYPOINT ["dotnet", "fttAssessment.dll"]