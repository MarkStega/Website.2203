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
COPY ["Website.2203/Website.Lib/Website.Lib.csproj", "Website.Lib/"]
COPY ["Website.2203/Website.WebAssembly/Website.WebAssembly.csproj", "Website.WebAssembly/"]
COPY ["Website.2203/Website.WebAssembly/NuGet.Config", "Website.WebAssembly/"]

RUN dotnet restore "/src/Website.WebAssembly" -s "https://api.nuget.org/v3/index.json"

RUN dotnet restore "Website.WebAssembly/Website.WebAssembly.csproj"

COPY . .
RUN dotnet build "Website.2203/Website.WebAssembly/Website.WebAssembly.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /src
RUN dotnet publish "Website.2203/Website.WebAssembly/Website.WebAssembly.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
#COPY Website.WebAssembly/nginx.conf /tmp/nginx.conf
#RUN rm /tmp/nginx.conf
#