#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TicketsManager.Api/TicketsManager.Api.csproj", "TicketsManager.Api/"]
COPY ["TicketsManager.BLL/TicketsManager.BLL.csproj", "TicketsManager.BLL/"]
COPY ["TicketsManager.DAL/TicketsManager.DAL.csproj", "TicketsManager.DAL/"]
COPY ["TicketsManager.Common/TicketsManager.Common.csproj", "TicketsManager.Common/"]
RUN dotnet restore "TicketsManager.Api/TicketsManager.Api.csproj"
COPY . .
WORKDIR "/src/TicketsManager.Api"
RUN dotnet build "TicketsManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicketsManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketsManager.Api.dll"]