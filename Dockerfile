﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Development
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore 
COPY . ./

RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "website_CLB_HTSV.dll"]