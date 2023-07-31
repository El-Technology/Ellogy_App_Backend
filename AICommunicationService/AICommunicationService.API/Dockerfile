#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AICommunicationService.API/AICommunicationService.Api.csproj", "AICommunicationService.API/"]
COPY ["AICommunicationService.BLL/AICommunicationService.BLL.csproj", "AICommunicationService.BLL/"]
COPY ["AICommunicationService.Common/AICommunicationService.Common.csproj", "AICommunicationService.Common/"]
RUN dotnet restore "AICommunicationService.API/AICommunicationService.Api.csproj"
COPY . .
WORKDIR "/src/AICommunicationService.API"
RUN dotnet build "AICommunicationService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AICommunicationService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AICommunicationService.Api.dll"]