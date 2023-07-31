FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AICommunicationService/AICommunicationService.csproj", "AICommunicationService/"]
RUN dotnet restore "AICommunicationService/AICommunicationService.csproj"
COPY . .
WORKDIR "/src/AICommunicationService"
RUN dotnet build "AICommunicationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AICommunicationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AICommunicationService.dll"]
