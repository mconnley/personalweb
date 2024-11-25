FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9000

ENV ASPNETCORE_URLS=http://+:9000

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["personalweb.csproj", "./"]
COPY ["NuGet.Config", "./"]
COPY ["rootca.crt", "/usr/local/share/ca-certificates/rootca.crt"]
RUN chmod 644 /usr/local/share/ca-certificates/rootca.crt && update-ca-certificates
RUN dotnet restore "personalweb.csproj" --configfile "NuGet.Config" -f
COPY . .
WORKDIR "/src/."
RUN dotnet build "personalweb.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "personalweb.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "personalweb.dll"]
