FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Practice.Gateway/Practice.Gateway.csproj", "Practice.Gateway/"]
RUN dotnet restore "Practice.Gateway/Practice.Gateway.csproj"

COPY . .
WORKDIR "/src/Practice.Gateway"
RUN dotnet build "Practice.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Practice.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Practice.Gateway.dll"]