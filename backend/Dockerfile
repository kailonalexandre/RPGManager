FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RpgManager.slnx ./
COPY src/RpgManager.Domain/RpgManager.Domain.csproj src/RpgManager.Domain/
COPY src/RpgManager.Application/RpgManager.Application.csproj src/RpgManager.Application/
COPY src/RpgManager.Infrastructure/RpgManager.Infrastructure.csproj src/RpgManager.Infrastructure/
COPY src/RpgManager.Api/RpgManager.Api.csproj src/RpgManager.Api/
COPY tests/RpgManager.Infrastructure.Tests/RpgManager.Infrastructure.Tests.csproj tests/RpgManager.Infrastructure.Tests/

RUN dotnet restore RpgManager.slnx
COPY . .
RUN dotnet publish src/RpgManager.Api/RpgManager.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "RpgManager.Api.dll"]
