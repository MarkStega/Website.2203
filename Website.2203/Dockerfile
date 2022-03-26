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
COPY ["Website.2203/Website.2203.csproj", "Website.2203/"]
COPY ["Website.2203/NuGet.Config", "Website.2203/"]

RUN dotnet restore "/src/Website.2203" -s "https://api.nuget.org/v3/index.json"

RUN dotnet restore "Website.2203/Website.2203.csproj"

COPY . .
WORKDIR "/src/Website.2203"
RUN dotnet build "Website.2203.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src"
RUN dotnet publish "Website.2203/Website.2203.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY Website.2203/nginx.conf /tmp/nginx.conf
#RUN iconv -f ISO-8859-1 -t UTF-8 /tmp/nginx.conf -o /etc/nginx/nginx.conf
#RUN iconv -f ISO-8859-1 -t UTF-8//TRANSLIT /tmp/nginx.conf -o /etc/nginx/nginx.conf
#RUN iconv -f ISO-8859-1 -t UTF-8 /tmp/nginx.conf > /etc/nginx/nginx.conf
RUN rm /tmp/nginx.conf
