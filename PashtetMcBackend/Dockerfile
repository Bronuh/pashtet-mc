FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BronuhMcBackend/BronuhMcBackend.csproj", "BronuhMcBackend/"]
RUN dotnet restore "BronuhMcBackend/BronuhMcBackend.csproj"
COPY . .
WORKDIR "/src/BronuhMcBackend"
RUN dotnet build "BronuhMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BronuhMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENV DirectorySettings__RootDirectory=/data
ENTRYPOINT ["dotnet", "BronuhMcBackend.dll"]
