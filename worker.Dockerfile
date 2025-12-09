#Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY worker ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

#Runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet","Q10.TaskManager.Worker.dll"]

