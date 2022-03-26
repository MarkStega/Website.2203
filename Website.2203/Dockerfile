#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

RUN apt-get update
RUN apt-get install --no-install-recommends --yes python3
RUN dotnet --info
RUN dotnet workload install microsoft-net-sdk-blazorwebassembly-aot --skip-manifest-update
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

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Website.2203.dll"]
#
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY Website.2203/nginx.conf /etc/nginx/nginx.conf




#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY Website.2203/Website.2203.csproj .
#RUN dotnet restore Website.2203.csproj
#COPY . .
#RUN dotnet build Website.2203.csproj -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish Website.2203.csproj -c Release -o /app/publish
#
##FROM nginx:alpine AS final
#WORKDIR /usr/share/nginx/html
#COPY --from=publish /app/publish/wwwroot .
#COPY Website.2203/nginx.conf /etc/nginx/nginx.conf