#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UserManager.Api/UserManager.Api.csproj", "UserManager.Api/"]
COPY ["UserManager.BLL/UserManager.BLL.csproj", "UserManager.BLL/"]
COPY ["UserManager.DAL/UserManager.DAL.csproj", "UserManager.DAL/"]
COPY ["UserManager.Common/UserManager.Common.csproj", "UserManager.Common/"]
RUN dotnet restore "UserManager.Api/UserManager.Api.csproj"
COPY . .
WORKDIR "/src/UserManager.Api"
RUN dotnet build "UserManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManager.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManager.Api.dll"]