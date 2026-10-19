FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY SmartX.Api/SmartX.Api.csproj SmartX.Api/
COPY SmartX.Api/HealthProbe/HealthProbe.csproj SmartX.Api/HealthProbe/
RUN dotnet restore SmartX.Api/SmartX.Api.csproj
RUN dotnet restore SmartX.Api/HealthProbe/HealthProbe.csproj

COPY SmartX.Api/ SmartX.Api/
RUN dotnet publish SmartX.Api/SmartX.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore
RUN dotnet publish SmartX.Api/HealthProbe/HealthProbe.csproj \
    -c Release \
    -o /app/healthprobe

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
COPY --from=build /app/healthprobe /healthprobe

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "SmartX.Api.dll"]
