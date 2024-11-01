FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
# Prepare sources
WORKDIR /src
COPY ["PashtetMcBackend", "PashtetMcBackend"]
COPY ["Common", "Common"]
COPY ["Launcher", "Launcher"]
COPY ["PashtetMc.sln", "PashtetMc.sln"]
RUN dotnet restore "PashtetMc.sln"
WORKDIR "/src/PashtetMcBackend"
RUN dotnet build "PashtetMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PashtetMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENV DirectorySettings__RootDirectory=/data
ENTRYPOINT ["dotnet", "PashtetMcBackend.dll"]
