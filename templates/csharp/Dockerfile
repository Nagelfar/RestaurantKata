FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["csharp.csproj", "./"]
RUN dotnet restore "csharp.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "csharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "csharp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "csharp.dll"]