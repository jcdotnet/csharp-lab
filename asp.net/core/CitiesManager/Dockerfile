FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["CitiesManager.WebAPI/CitiesManager.WebAPI.csproj", "CitiesManager.WebAPI/"]
COPY ["CitiesManager.Core/CitiesManager.Core.csproj", "CitiesManager.Core/"]
COPY ["CitiesManager.Infrastructure/CitiesManager.Infrastructure.csproj", "CitiesManager.Infrastructure/"]

RUN dotnet restore "CitiesManager.WebAPI/CitiesManager.WebAPI.csproj"
COPY . .
WORKDIR "/src/CitiesManager.WebAPI"
RUN dotnet build "CitiesManager.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CitiesManager.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CitiesManager.WebAPI.dll"]