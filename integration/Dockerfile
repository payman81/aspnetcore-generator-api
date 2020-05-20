# Build Stage
FROM mcr.microsoft.com/dotnet/core/sdk:3.1  AS build-env
WORKDIR /integration

COPY integration.csproj .
RUN dotnet restore

COPY . .

ENTRYPOINT ["dotnet", "test"]