#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Website.2203/Website.Lib/Website.Lib.csproj", "Website.2203/Website.Lib/"]
COPY ["Website.2203/Website.BlazorServer/Website.BlazorServer.csproj", "Website.2203/Website.BlazorServer/"]
COPY ["Website.2203/Website.BlazorServer/NuGet.Config", "Website.2203/Website.BlazorServer/"]

RUN dotnet restore "Website.2203/Website.BlazorServer/Website.BlazorServer.csproj"

COPY . .
WORKDIR "/src/Website.2203/Website.BlazorServer"
RUN dotnet build "Website.BlazorServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Website.BlazorServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Website.BlazorServer.dll"]