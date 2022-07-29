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
RUN apt-get install --no-install-recommends --yes apt-utils python3 npm
RUN dotnet --info
RUN dotnet workload install wasm-tools

WORKDIR /src
COPY ["Website.BlazorWebAssembly/Server/Website.BlazorWebAssembly.Server.csproj", "Website.BlazorWebAssembly/Server/"]
COPY ["Website.BlazorWebAssembly/Server/NuGet.Config", "Website.BlazorWebAssembly/Server/"]

COPY ["Website.BlazorWebAssembly/Client/Website.BlazorWebAssembly.Client.csproj", "Website.BlazorWebAssembly/Client/"]
COPY ["Website.Lib/Website.Lib.csproj", "Website.Lib/"]
COPY ["Website.ServerLib/Website.ServerLib.csproj", "Website.ServerLib/"]

RUN dotnet restore "Website.BlazorWebAssembly/Server/Website.BlazorWebAssembly.Server.csproj"
COPY . .
WORKDIR "/src/Website.BlazorWebAssembly/Server"
RUN dotnet publish "Website.BlazorWebAssembly.Server.csproj" -c Release -o /app/publish -p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Website.BlazorWebAssembly.Server.dll"]