FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartX.Api/SmartX.Api.csproj SmartX.Api/
RUN dotnet restore SmartX.Api/SmartX.Api.csproj

COPY SmartX.Api/ SmartX.Api/
RUN dotnet publish SmartX.Api/SmartX.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartX.Api.dll"]
