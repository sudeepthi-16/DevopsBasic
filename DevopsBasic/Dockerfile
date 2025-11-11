# ========================
# Backend Dockerfile (.NET 8)
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "DevopsBasic.dll"]


# # Build stage
# FROM ghcr.io/microsoft/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY . .
# RUN dotnet restore
# RUN dotnet publish -c Release -o /app/publish

# # Runtime stage
# FROM ghcr.io/microsoft/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app
# COPY --from=build /app/publish .
# ENTRYPOINT ["dotnet", "DevopsBasic.dll"]
