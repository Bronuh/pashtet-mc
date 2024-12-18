FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN pwd
COPY ["./PashtetMcBackend/", "./PashtetMcBackend/"]
COPY ["./Common/", "./Common/"]
COPY ["./Launcher/", "./Launcher/"]
COPY ["./PashtetMc.sln", "./"]
RUN ls -al
WORKDIR "/src/PashtetMcBackend"
RUN dotnet publish "PashtetMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
VOLUME /storage
VOLUME /mcserver
USER 1001
ENTRYPOINT ["dotnet", "PashtetMcBackend.dll"]
