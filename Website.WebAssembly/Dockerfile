#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

RUN apt-get update
RUN apt-get install --no-install-recommends --yes python3
RUN dotnet --info
RUN dotnet workload install wasm-tools

WORKDIR /src
COPY ["Website.2203/Website.Lib/Website.Lib.csproj", "Website.2203/Website.Lib/"]
COPY ["Website.2203/Website.WebAssembly/Website.WebAssembly.csproj", "Website.2203/Website.WebAssembly/"]
COPY ["Website.2203/Website.WebAssembly/NuGet.Config", "Website.2203/Website.WebAssembly/"]

RUN dotnet restore "/src/Website.2203/Website.WebAssembly" -s "https://api.nuget.org/v3/index.json"

RUN dotnet restore "Website.2203/Website.WebAssembly/Website.WebAssembly.csproj"

COPY . .
WORKDIR "/src/Website.2203/Website.WebAssembly"
RUN dotnet build "Website.WebAssembly.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Website.2203/Website.WebAssembly"
RUN dotnet publish "Website.WebAssembly.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY Website.2203/Website.WebAssembly/nginx.conf /tmp/nginx.conf
RUN rm /tmp/nginx.conf
