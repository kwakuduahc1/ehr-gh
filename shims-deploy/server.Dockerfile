# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app
COPY server /app
ENTRYPOINT ["dotnet", "ShimsServer.dll"]
