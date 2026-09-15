FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

#Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Agenda/Agenda.csproj", "Agenda/"]
COPY ["Agenda-Test/Agenda-Test.csproj", "Agenda-Test/"]

RUN dotnet restore "Agenda/Agenda.csproj"
RUN dotnet restore "Agenda-Test/Agenda-Test.csproj"

COPY Agenda/ Agenda/
COPY Agenda-Test/ Agenda-Test/

RUN dotnet build "Agenda/Agenda.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS test
WORKDIR /src

RUN dotnet tool install --global dotnet-reportgenerator-globaltool
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet test "Agenda-Test/Agenda-Test.csproj" \
    --no-restore \
    --collect:"XPlat Code Coverage" \
    --logger "trx;LogFileName=TestResults.trx" \
    --results-directory /app/test-results \
    -c Release 

RUN reportgenerator \
    -reports:"/app/test-results/**/coverage.cobertura.xml" \
    -targetdir:/app/test-results/html \
    -reporttypes:Html 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Agenda/Agenda.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM nginx:latest AS testweb

RUN rm -rf /usr/share/nginx/html/*

COPY --from=test /app/test-results/html /usr/share/nginx/html

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Agenda.dll"]
