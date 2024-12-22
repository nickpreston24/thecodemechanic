FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src
COPY ["thecodemechanic.csproj", "./"]
COPY ["nuget.config", "./"]
RUN dotnet restore "thecodemechanic.csproj"
COPY . .
WORKDIR "/src/"
RUN libman restore
RUN dotnet build "thecodemechanic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "thecodemechanic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "thecodemechanic.dll"]
