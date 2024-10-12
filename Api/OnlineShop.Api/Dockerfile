#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OnlineShop/OnlineShop.Api/OnlineShop.Api.csproj", "OnlineShop/OnlineShop.Api/"]
COPY ["OnlineShop/OnlineShop.BLL/OnlineShop.BLL.csproj", "OnlineShop/OnlineShop.BLL/"]
COPY ["OnlineShop/OnlineShop.DataLayer/OnlineShop.DataLayer.csproj", "OnlineShop/OnlineShop.DataLayer/"]
RUN dotnet restore "./OnlineShop/OnlineShop.Api/OnlineShop.Api.csproj"
COPY . .
WORKDIR "/src/OnlineShop/OnlineShop.Api"
RUN dotnet build "./OnlineShop.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OnlineShop.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineShop.Api.dll"]