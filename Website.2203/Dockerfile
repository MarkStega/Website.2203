##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
#WORKDIR /src
#RUN dotnet workload install wasm-tools
#COPY ["2022-03-Website/2022-03-Website.csproj", "2022-03-Website/2022-03-Website/"]
#COPY ["2022-03-Website/NuGet.Config", "2022-03-Website/2022-03-Website/"]
#
#RUN dotnet restore "/src/2022-03-Website/2022-03-Website" -s "https://api.nuget.org/v3/index.json"
#
#RUN dotnet restore "2022-03-Website/2022-03-Website/2022-03-Website.csproj"
#
#COPY . .
#WORKDIR "/src/2022-03-Website/2022-03-Website"
#RUN dotnet build "2022-03-Website.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "2022-03-Website.csproj" -c Release -o /app/publish
#
##FROM base AS final
##WORKDIR /app
##COPY --from=publish /app/publish .
##ENTRYPOINT ["dotnet", "2022-03-Website.dll"]
#
##FROM nginx:alpine AS final
##WORKDIR /usr/share/nginx/html
##COPY --from=publish /app/publish/wwwroot .
##COPY nginx.conf /etc/nginx/nginx.conf




FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY Website.2203.csproj .
RUN dotnet restore Website.2203.csproj
COPY . .
RUN dotnet build Website.2203.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Website.2203.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf