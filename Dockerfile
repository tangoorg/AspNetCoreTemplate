FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /source
COPY . .

RUN dotnet restore AspNetCoreTemplate.sln
RUN dotnet publish AspNetCoreTemplate/AspNetCoreTemplate.csproj -c Release -o /app/published-app /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

COPY --from=build /app/published-app ./

ENTRYPOINT ["dotnet", "AspNetCoreTemplate.dll"]
