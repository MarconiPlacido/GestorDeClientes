FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

WORKDIR /src/GestorDeClientes.Worker

RUN dotnet restore GestorDeClientes.Worker.csproj
RUN dotnet publish GestorDeClientes.Worker.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GestorDeClientes.Worker.dll"]
