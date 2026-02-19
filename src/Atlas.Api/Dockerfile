# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/Atlas.Domain/Atlas.Domain.csproj src/Atlas.Domain/
COPY src/Atlas.Application/Atlas.Application.csproj src/Atlas.Application/
COPY src/Atlas.Infrastructure/Atlas.Infrastructure.csproj src/Atlas.Infrastructure/
COPY src/Atlas.Api/Atlas.Api.csproj src/Atlas.Api/

RUN dotnet restore src/Atlas.Api/Atlas.Api.csproj

COPY src ./src
RUN dotnet publish src/Atlas.Api/Atlas.Api.csproj -c Release -o /out --no-restore

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Atlas.Api.dll"]
