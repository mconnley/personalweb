FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy AS base
WORKDIR /app
EXPOSE 9000

ENV ASPNETCORE_URLS=http://+:9000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
RUN mkdir /var/log/personalweb && chown -R appuser /var/log/personalweb
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-jammy AS build
WORKDIR /src
COPY ["personalweb.csproj", "./"]
COPY ["NuGet.Config", "./"]
RUN dotnet restore "personalweb.csproj" --configfile "NuGet.Config" -f -v detailed
COPY . .
WORKDIR "/src/."
RUN dotnet build "personalweb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "personalweb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "personalweb.dll"]
