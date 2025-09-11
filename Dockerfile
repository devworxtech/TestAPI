FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["LogTestAPI/LogTestAPI.csproj", "LogTestAPI/"]
RUN dotnet restore "LogTestAPI/LogTestAPI.csproj"
COPY . .
WORKDIR "/src/LogTestAPI"
RUN dotnet build "LogTestAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LogTestAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "LogTestAPI.dll"]
