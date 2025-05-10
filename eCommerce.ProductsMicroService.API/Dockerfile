FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["eCommerce.ProductsMicroService.API/eCommerce.ProductsMicroService.API.csproj", "eCommerce.ProductsMicroService.API/"]
RUN dotnet restore "eCommerce.ProductsMicroService.API/eCommerce.ProductsMicroService.API.csproj"
COPY . .
WORKDIR "/src/eCommerce.ProductsMicroService.API"
RUN dotnet build "eCommerce.ProductsMicroService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "eCommerce.ProductsMicroService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV MYSQL_HOST=localhost
ENV MYSQL_PASSWORD=admin
ENTRYPOINT ["dotnet", "eCommerce.ProductsMicroService.API.dll"]
