FROM mcr.microsoft.com/dotnet/core/sdk:3.1  AS build-env

WORKDIR /generator

# restore
COPY api/api.csproj api/
COPY tests/tests.csproj tests/
COPY generator.sln .
RUN dotnet restore generator.sln

#copy source
COPY . .

#test - xunit env for better TC reporting
ENV TEAMCITY_PROJECT_NAME=fake  
RUN dotnet test tests/tests.csproj

#publish
RUN dotnet publish api/api.csproj -o /publish

#runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
COPY --from=build-env /publish /publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "api.dll"]