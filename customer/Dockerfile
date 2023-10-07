FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Customer.csproj", "./"]
RUN dotnet restore "Customer.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Customer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.dll"]