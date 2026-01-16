# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy source
COPY . .

# Restore
RUN dotnet restore NflTickerDemo/SportsTickerDemo.csproj

# Publish
RUN dotnet publish NflTickerDemo/SportsTickerDemo.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose default port
EXPOSE 8080

# Configure Kestrel to listen on 8080
ENV ASPNETCORE_URLS=http://+:8080

# Entry point
ENTRYPOINT ["dotnet","SportsTickerDemo.dll"]
