#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["sandbox/Sample.DotRealDb.Web/Server/Sample.DotRealDb.Web.Server.csproj", "sandbox/Sample.DotRealDb.Web/Server/"]
COPY ["sandbox/Sample.DotRealDb.Web/Client/Sample.DotRealDb.Blazor.csproj", "sandbox/Sample.DotRealDb.Web/Client/"]
COPY ["src/DotRealDb.Client/DotRealDb.Client.csproj", "src/DotRealDb.Client/"]
COPY ["sandbox/Sample.DotRealDb.Web/Shared/Sample.DotRealDb.Web.Shared.csproj", "sandbox/Sample.DotRealDb.Web/Shared/"]
COPY ["src/DotRealDb.AspNetCore/DotRealDb.AspNetCore.csproj", "src/DotRealDb.AspNetCore/"]
RUN dotnet restore "sandbox/Sample.DotRealDb.Web/Server/Sample.DotRealDb.Web.Server.csproj"
COPY . .
WORKDIR "/src/sandbox/Sample.DotRealDb.Web/Server"
RUN dotnet build "Sample.DotRealDb.Web.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.DotRealDb.Web.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Sample.DotRealDb.Web.Server.dll"]

CMD ASPNETCORE_URLS=http://*:$PORT dotnet Sample.DotRealDb.Web.Server.dll