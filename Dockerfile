FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Q10.TaskManager.Api/Q10.TaskManager.Api.csproj", "Q10.TaskManager.Api/"]
RUN dotnet restore "Q10.TaskManager.Api/Q10.TaskManager.Api.csproj"
COPY . .
WORKDIR "/src/Q10.TaskManager.Api"
RUN dotnet build "Q10.TaskManager.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Q10.TaskManager.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Q10.TaskManager.Api.dll"]
