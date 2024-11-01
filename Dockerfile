FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#USER $APP_UID
ARG BUILD_CONFIGURATION=Release
# Prepare sources
WORKDIR /src
RUN pwd
COPY ["./PashtetMcBackend/", "./PashtetMcBackend/"]
COPY ["./Common/", "./Common/"]
COPY ["./Launcher/", "./Launcher/"]
COPY ["./PashtetMc.sln", "./"]
RUN ls -al
WORKDIR "/src/PashtetMcBackend"
#RUN dotnet restore "PashtetMcBackend.csproj"
#RUN dotnet build "PashtetMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "PashtetMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#RUN pwd

#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN echo SOME ECHO
#WORKDIR "/src/PashtetMcBackend"
#RUN pwd
#RUN ls -al
#RUN dotnet publish "PashtetMcBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
#USER $APP_UID
WORKDIR /app
COPY --from=build /app/publish .
#RUN mkdir /storage
#RUN mkdir /mcserver
VOLUME /storage
VOLUME /mcserver
ENV UID=1000 GID=1000
#ENV DirectorySettings__RootDirectory=/data
ENTRYPOINT ["dotnet", "PashtetMcBackend.dll"]
