#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG version
ARG build-time
ENV VERSION $version
ENV BUILD_TIME $build-time

ENV DEBIAN_FRONTEND noninteractive
ENV DEBCONF_NOWARNINGS="yes"
RUN apt-get update
RUN apt-get install --no-install-recommends --yes apt-utils npm

WORKDIR /src
COPY ["Website.BlazorServer/Website.BlazorServer.csproj", "Website.BlazorServer/"]
COPY ["Website.BlazorServer/NuGet.Config", "Website.BlazorServer/"]

COPY ["Website.Lib/Website.Lib.csproj", "Website.Lib/"]
COPY ["Website.ServerLib/Website.ServerLib.csproj", "Website.ServerLib/"]

RUN dotnet restore --disable-parallel "Website.BlazorServer/Website.BlazorServer.csproj"
COPY . .
WORKDIR "/src/Website.BlazorServer"
RUN dotnet publish "Website.BlazorServer.csproj" -c Release -o /app/publish -p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Website.BlazorServer.dll"]