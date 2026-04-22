FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PortalAcademico.csproj", "./"]
RUN dotnet restore "PortalAcademico.csproj"
COPY . .
RUN dotnet publish "PortalAcademico.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PortalAcademico.dll"]